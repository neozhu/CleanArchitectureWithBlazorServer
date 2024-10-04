export function getTimezoneOffset() {
    return new Date().getTimezoneOffset() / 60;
}

export function getTimezoneOffsetByTimeZone(timezone) {
    const date = new Date();
    const tzDate = new Date(date.toLocaleString('en-US', { timeZone: timezone }));
    const offset = (date.getTime() - tzDate.getTime()) / 60;
    return offset;
}