import { AuthorsTable } from "./components/authrosTable";
import { ConcatMapButton, MergeMapButton, SwitchMapButton } from "./components/buttons";
import { Form } from "./components/form";
import { GetError } from "./components/getError";
import { Search } from "./components/search";

const appRoot = document.getElementById("app");

const authorsTable = new AuthorsTable("/dane.json");
authorsTable.mount(appRoot);

const form = new Form();
form.mount(appRoot);

const getError = new GetError();
getError.mount(appRoot);
getError.oneError();
getError.repeatError();

const search = new Search();
search.mount(appRoot);

const switchButton = new SwitchMapButton();
switchButton.mount(appRoot);

const mergeButton = new MergeMapButton();
mergeButton.mount(appRoot);

const concatButton = new ConcatMapButton();
concatButton.mount(appRoot);