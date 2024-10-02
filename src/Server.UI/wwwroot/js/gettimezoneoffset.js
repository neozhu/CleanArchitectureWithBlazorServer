export function getTimezoneOffset(timezone) {
    console.log(timezone);
    if (timezone) {
        const date = new Date();
        // Get the time in the specified timezone
        const timeInTimeZone = new Date(date.toLocaleString("en-US", { timeZone: timezone }));
        // Get the time in UTC
        const timeInUTC = new Date(date.toLocaleString("en-US", { timeZone: 'UTC' }));
        // Calculate the difference between the specified timezone and UTC
        const offset = (timeInTimeZone.getTime() - timeInUTC.getTime()) / 1000 / 60 / 60;
        return offset;
    } else {
        // Default to returning the local timezone offset
        return - (new Date().getTimezoneOffset()) / 60;
    }
}