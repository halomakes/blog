export class ImmichGallery {
    public static readonly selector = 'immich-gallery';

    constructor(element: HTMLElement) {
        const frame = element as HTMLIFrameElement;
        frame.addEventListener('load', ev => {
            const style = document.createElement("style");
            style.textContent = "header { display: none!important; } main {padding-top: 0!important}";
            (ev.target as any).contentDocument.head.appendChild(style);
        })
    }
}