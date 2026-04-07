import hljs from 'highlight.js/lib/core';
import javascript from 'highlight.js/lib/languages/javascript';
import powershell from 'highlight.js/lib/languages/powershell';

const languageMap = new Map<string, any>([
    ['javascript', javascript],
    ['js', javascript],
    ['powershell', powershell],
    ['pwsh', powershell],
    ['ps1', powershell],
]);

export class CodeSnippet {
    public static readonly selector: string = '.code-snippet';
    private static initializedLanguages: string[] = [];
    private code?: string;

    constructor(element: HTMLElement) {
        const codeElement: HTMLElement = element.lastElementChild as HTMLElement;
        if (!codeElement)
            return;
        this.code = codeElement.innerText;
        const language: string = (codeElement as any).lang;
        this.highlightCode(language, codeElement);
        this.setupCopyButton(element);
    }

    private highlightCode(language: string, codeElement: HTMLElement) {
        if (!language)
            return;
        if (!languageMap.has(language))
            return;
        if (!CodeSnippet.initializedLanguages.includes(language)) {
            CodeSnippet.initializedLanguages.push(language);
            hljs.registerLanguage(language, languageMap.get(language));
        }

        hljs.highlightElement(codeElement);
    }

    private setupCopyButton(element: HTMLElement) {
        const button = element.querySelector('[data-action=copy]');
        if (!button)
            return;
        button.addEventListener('click', () => this.copy());
    }

    public async copy() {
        if (this.code) {
            await navigator.clipboard.writeText(this.code);
        }
    }
}