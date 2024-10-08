document.addEventListener("DOMContentLoaded", function () {
    addEvent(agendas)
        .then(() => {
            console.log("All events have been added.");
            // You can add any additional logic here that should run after the events are added.
        })
        .catch(error => {
            console.error("Error adding events:", error);
        });
});

function addEvent(agendas) {
    return new Promise((resolve, reject) => {
        if (!agendas || !Array.isArray(agendas) || agendas.length === 0) {
            console.error("Agendas is not a valid array or is empty:", agendas);
            reject("Invalid agendas");
            return;
        }
        console.log("Agendas:", agendas);

        // Group the agendas by day
        var agendasByDay = {};
        agendas.forEach(function (agenda) {
            var date = agenda.StartDateTime.split('T')[0]; // Get the day part of the date
            if (!agendasByDay[date]) {
                agendasByDay[date] = [];
            }
            agendasByDay[date].push(agenda);
        });

        // Find the overall earliest start time across all days
        var globalEarliestStartTime = findGlobalEarliestStartTime(agendas);
        console.log("Global earliest start time:", globalEarliestStartTime);

        // Find the latest end time across all days for proper column height adjustment
        var globalLatestEndTime = findGlobalLatestEndTime(agendas);
        console.log("Global latest end time:", globalLatestEndTime);

        // Adjust the height of each schedule column to fit all events
        var totalMinutes = (globalLatestEndTime - globalEarliestStartTime);
        var containerHeight = (totalMinutes / 75) * 75; // Calculate new height based on the time range

        if (containerHeight > 600) {
            document.querySelectorAll('.day-column').forEach(function (column) {
                column.style.height = `${containerHeight}px`; // Set new height for the column
            });
        }

        // Process each day
        Object.keys(agendasByDay).forEach(function (date) {
            var dayAgendas = agendasByDay[date];
            var overlappingGroups = findOverlappingGroups(dayAgendas);

            // Process each overlapping group
            overlappingGroups.forEach(function (group) {
                var groupSize = group.length; // How many events overlap in this group?

                group.forEach(function (agenda, index) {
                    var startTime = new Date(agenda.StartDateTime);
                    var endTime = new Date(agenda.EndDateTime);

                    // Check if the Subject is null
                    var subjectName = agenda.SubjectDto ? agenda.SubjectDto.SubjectName : 'No Subject Assigned';
                    console.log("Subject:", subjectName, "Start:", startTime, "End:", endTime);

                    // Create a new div for the agenda item
                    var scheduleItem = document.createElement('div');
                    scheduleItem.className = 'schedule-item';
                    scheduleItem.innerHTML = `
                                <p>${subjectName}</p>
                                <p>${startTime.getHours()}:${startTime.getMinutes().toString().padStart(2, '0')} -
                                   ${endTime.getHours()}:${endTime.getMinutes().toString().padStart(2, '0')}</p>
                            `;

                    // Dynamically set the width and left positioning based on how many events are in this group
                    var widthPercentage = 100 / groupSize; // Divide the width by the number of overlapping events
                    scheduleItem.style.width = `calc(${widthPercentage}% - 10px)`; // Adjust for padding
                    scheduleItem.style.left = `${index * widthPercentage}%`; // Position it based on its index in the group

                    // Adjust top position based on the start time relative to the global earliest start time
                    var totalMinutesFromEarliest = (startTime.getHours() * 60 + startTime.getMinutes()) - globalEarliestStartTime; // Minutes difference from earliest
                    console.log("Minutes from earliest:", totalMinutesFromEarliest);
                    var moduleHeight = 75; // Each module is 75px high
                    scheduleItem.style.top = `${(totalMinutesFromEarliest / 75) * moduleHeight}px`; // Adjust top positioning
                    console.log(`Setting height for ${date} to ${containerHeight}px`);

                    // Append the new item to the corresponding day column
                    var dayColumn = document.querySelector(`[data-date="${date}"]`);
                    if (dayColumn) {
                        dayColumn.appendChild(scheduleItem);
                    }
                });
            });
        });

        // Resolve the promise once all events have been processed
        resolve();
    });
}

// Function to find the global earliest start time across all agendas
function findGlobalEarliestStartTime(agendas) {
    let earliest = null;

    agendas.forEach(function (agenda) {
        const startTime = new Date(agenda.StartDateTime);

        if (isNaN(startTime.getTime())) {
            console.error("Invalid date encountered:", agenda.StartDateTime);
            return;
        }

        // Create a comparable time value
        const currentComparableTime = startTime.getHours() * 60 + startTime.getMinutes();

        // Compare against the earliest time found
        if (earliest === null || currentComparableTime < earliest) {
            earliest = currentComparableTime; // Store the earliest comparable time
        }
    });

    return earliest; // Return the overall earliest start time as an integer (in minutes)
}

// Function to find the global latest end time across all agendas based on time only
function findGlobalLatestEndTime(agendas) {
    let latest = null;

    agendas.forEach(function (agenda) {
        const endTime = new Date(agenda.EndDateTime);

        if (isNaN(endTime.getTime())) {
            console.error("Invalid date encountered:", agenda.EndDateTime);
            return;
        }

        // Create a comparable time value
        const currentComparableTime = endTime.getHours() * 60 + endTime.getMinutes();

        // Compare against the latest time found
        if (latest === null || currentComparableTime > latest) {
            latest = currentComparableTime; // Store the latest comparable time
        }
    });

    return latest; // Return the overall latest end time as an integer (in minutes)
}
// Function to find overlapping groups of agendas for each day
function findOverlappingGroups(dayAgendas) {
    var overlappingGroups = [];
    dayAgendas.sort((a, b) => new Date(a.StartDateTime) - new Date(b.StartDateTime));

    var currentGroup = [dayAgendas[0]];
    for (var i = 1; i < dayAgendas.length; i++) {
        var prevAgenda = dayAgendas[i - 1];
        var currentAgenda = dayAgendas[i];

        var prevEndTime = new Date(prevAgenda.EndDateTime);
        var currentStartTime = new Date(currentAgenda.StartDateTime);

        // Check if current agenda overlaps with the previous one
        if (currentStartTime < prevEndTime) {
            currentGroup.push(currentAgenda);
        } else {
            overlappingGroups.push(currentGroup);
            currentGroup = [currentAgenda];
        }
    }

    // Add the last group
    overlappingGroups.push(currentGroup);
    return overlappingGroups;
}