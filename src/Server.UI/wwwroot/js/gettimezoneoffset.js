export function getTimezoneOffset() {
    return Math.round(-new Date().getTimezoneOffset() / 60);
}

export function getTimezoneOffsetByTimeZone(timezone) {
    // Create a Date object representing the current time
    const now = new Date();

    // Use Intl.DateTimeFormat to format the current date with the specified timezone
    const tzFormatter = new Intl.DateTimeFormat('en-US', {
        timeZone: timezone,
        timeZoneName: 'short' // Include the timezone name, such as GMT+8 or GMT-4
    });

    // Format the date and extract the timezone name part
    const tzParts = tzFormatter.formatToParts(now);
    const timeZoneName = tzParts.find(part => part.type === 'timeZoneName').value;

    // Use a regular expression to extract the offset from the timezone name (e.g., GMT+8, GMT-4:30)
    const offsetMatch = timeZoneName.match(/([+-]\d{1,2})(?::(\d{2}))?/);
    if (offsetMatch) {
        // Parse hours and optional minutes from the matched string
        const hours = parseInt(offsetMatch[1], 10);
        const minutes = offsetMatch[2] ? parseInt(offsetMatch[2], 10) : 0;

        // Calculate total offset in hours, considering minutes as part of the hour
        // If hours are negative, minutes should also contribute negatively
        return hours + (minutes / 60) * (hours < 0 ? -1 : 1);
    }

    // If no offset information is found, return 0 as a default
    return 0;
}