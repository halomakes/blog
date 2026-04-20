export class CodeSnippet {
    public static readonly selector: string = '.code-snippet';
    private codeElement?: HTMLElement;

    constructor(element: HTMLElement) {
        this.codeElement = element.lastElementChild as HTMLElement;
        if (!this.codeElement)
            return;
        this.setupCopyButton(element);
    }

    private setupCopyButton(element: HTMLElement) {
        const button = element.querySelector('[data-action=copy]');
        if (!button)
            return;
        button.addEventListener('click', (e) => this.copy(e as MouseEvent));
    }

    public async copy(e: MouseEvent) {
        e.preventDefault();
        if (this.codeElement) {
            await navigator.clipboard.writeText(this.codeElement.outerText);
        }
    }
}