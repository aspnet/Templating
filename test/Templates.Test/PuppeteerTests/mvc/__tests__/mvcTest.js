const puppeteer = require('puppeteer');
const os = require("os");
const hostname = os.hostname();

const serverPath = `https://localhost:5001`;

// e.g., npm test --debug
// In debug mode we show the editor, slow down operations, and increase the timeout for each test
const debug = process.env.npm_config_debug || false;
jest.setTimeout(debug ? 60000 : 30000);

let browser;

beforeAll(async () => {
    browser = await puppeteer.launch(debug ? { ignoreHTTPSErrors: true, headless: false, slowMo: 100 } : { ignoreHTTPSErrors: true });
    page = await browser.newPage();
});

afterAll(async () => {
    if (browser) {
        await browser.close();
    }
});

describe('mvc pages are ok', () => {
    it('index page works', async () => {
        await page.goto(serverPath);
        await page.waitFor('h1');
        heading = await page.$eval('h1', heading => heading.innerText);
        expect(heading).toBe('Welcome');
    });

    it('fails on missing', async () => {
        path = `${serverPath}/Falso`;
        console.log(path);

        const response = await page.goto(path);
        expect(response.status()).toBe(404);
    });
})
