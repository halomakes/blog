import './theme.scss'
import {CodeSnippet} from "./components/code-snippet.ts";
import {loadIcons} from "./components/icons.ts";
import {Selector} from "./components/selector..ts";
import {ImmichGallery} from "./components/immich-gallery.ts";
import { Navbar } from "./components/navbar.ts";

declare let window: { components: any[] };

const ready = (fn: () => any): void => {
    if (document.readyState !== 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
}


/**
 * Initialize all components on a page by their selectors
 * @param type The type to set up
 */
const initializeComponent = <TComponent>(type: {
    new(element: HTMLElement): TComponent,
    selector: string
}): TComponent[] => {
    const elements = document.querySelectorAll(type.selector);
    return Array.from(elements).map(el => el as HTMLElement).map(el => new type(el));
}

ready(() => {
    window.components = [
        ...initializeComponent(CodeSnippet),
        ...initializeComponent(Selector),
        ...initializeComponent(ImmichGallery),
        ...initializeComponent(Navbar)
    ];
    loadIcons();
});
