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
    public static readonly selector: string = 'code';
    private static initializedLanguages: string[] = [];

    constructor(element: HTMLElement) {
        const language: string = element.lang;
        if (!language)
            return;
        if (!languageMap.has(language))
            return;
        if (!CodeSnippet.initializedLanguages.includes(language)) {
            CodeSnippet.initializedLanguages.push(language);
            hljs.registerLanguage(language, languageMap.get(language));
        }

        hljs.highlightElement(element);
    }
}