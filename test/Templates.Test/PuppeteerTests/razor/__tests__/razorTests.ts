import { Page, Browser, launch } from 'puppeteer';
import { bindConsole, validateMessages } from '../../testFuncs/testFuncs';

const serverPath = `https://localhost:1234`;

jest.setTimeout(30000);

let browser: Browser = null;
let page: Page = null;
let badMessages: string[] = [];

beforeAll(async () => {
    browser = await launch({ ignoreHTTPSErrors: true });
    page = await browser.newPage();
    badMessages = bindConsole(page);
});

afterAll(async () => {
    if (browser) {
        await browser.close();
    }
});
