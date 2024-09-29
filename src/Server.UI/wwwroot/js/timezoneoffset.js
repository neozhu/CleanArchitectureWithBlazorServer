export function getTimeZoneOffset() {
    return (new Date().getTimezoneOffset()) / 60;
}