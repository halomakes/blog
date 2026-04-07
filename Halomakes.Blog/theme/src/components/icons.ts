import {
    createIcons,
    Download,
    ClipboardCopy,
    ChevronsDown
} from 'lucide';

export function loadIcons() {
    createIcons({
        icons: {
            Download,
            ClipboardCopy,
            ChevronsDown
        }
    });
}
