export class Navbar {
    public static readonly selector = 'nav';

    constructor(element: HTMLElement) {
        const hostSegments = window.location.host.split('.');
        if (hostSegments.length < 2)
            return;
        Array.from(element.querySelectorAll('.domain')).forEach(el => el.textContent = hostSegments[hostSegments.length - 2])
        Array.from(element.querySelectorAll('.tld')).forEach(el => el.textContent = hostSegments[hostSegments.length - 1])
    }
}