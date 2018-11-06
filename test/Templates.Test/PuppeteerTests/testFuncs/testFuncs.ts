import { Page } from 'puppeteer';

export function bindConsole(page: Page): string[] {
    let badTypes = ['error', 'warning'];
    let messages: string[] = [];
    page.on('console', msg => {
        if (badTypes.indexOf(msg.type()) > -1) {
            messages.push(msg.text());
        }
    });

    return messages;
}

export function maybeValidateIdentity(serverPath: string): void {
    // TODO: validate identity here in the future
}

export function validateMessages(messages: string[]): void {
    if (messages.length > 0) {
        fail(messages);
    }
}
