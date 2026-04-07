export class Selector {
    public static readonly selector: string = '.selector';
    private element: HTMLElement;
    private static readonly activeClass: string = 'open';

    constructor(element: HTMLElement) {
        this.element = element;
        this.element.querySelector('a.selected')?.addEventListener('click', e => this.toggle(e as MouseEvent));
    }

    toggle(event: MouseEvent) {
        event.preventDefault();
        this.element.classList.toggle(Selector.activeClass);
    }
}