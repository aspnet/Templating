const puppeteer = require('puppeteer');
const os = require("os");
const hostname = os.hostname();

const serverPath = `http://${hostname}:9000`;

// e.g., npm test --debug
// In debug mode we show the editor, slow down operations, and increase the timeout for each test
const debug = process.env.npm_config_debug || false;
jest.setTimeout(debug ? 60000 : 30000);

let browser;

beforeAll(async () => {
    const options = debug ?
        { headless: false, slowMo: 100 } :
        { args: ['--no-sandbox'] };
    browser = await puppeteer.launch(options);
    expect(browser).toBeDefined();
});

afterAll(async () => {
    await browser.close();
});

describe('mvc pages are ok', () => {
    const testPagePath = `http://${hostname}:9001/`;
    let page;

    beforeAll(async () => {
        page = await browser.newPage();
        await page.goto(testPagePath);
    });

    test('index page works', async () => {
        const result = await page.evaluate(async (serverPath) => {
            const url = ``
        }
    });
})
