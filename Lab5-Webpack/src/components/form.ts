import { fromEvent } from "rxjs";
import { ajax } from "rxjs/ajax";
import { Component } from "./component";

interface FormPayload {
    name: string;
    surname: string;
    email: string;
}

export class Form extends Component {
    private _form: HTMLFormElement;
    private _nameInput: HTMLInputElement;
    private _surnameInput: HTMLInputElement;
    private _emailInput: HTMLInputElement;
    private _submitButton: HTMLButtonElement;

    constructor() {
        super();

        this._form = document.createElement("form");

        this._nameInput = document.createElement("input");
        this._nameInput.type = "text";
        this._nameInput.name = "name";
        this._nameInput.placeholder = "Name";

        this._surnameInput = document.createElement("input");
        this._surnameInput.type = "text";
        this._surnameInput.name = "surname";
        this._surnameInput.placeholder = "Surname";

        this._emailInput = document.createElement("input");
        this._emailInput.type = "email";
        this._emailInput.name = "email";
        this._emailInput.placeholder = "Email";

        this._submitButton = document.createElement("button");
        this._submitButton.type = "submit";
        this._submitButton.innerText = "Submit";

        this._form.append(
            this._nameInput,
            this._surnameInput,
            this._emailInput,
            this._submitButton
        );

        this.appendChild(this._form);

        fromEvent<SubmitEvent>(this._form, "submit").subscribe((event) => {
            event.preventDefault();

            const payload = {
                name: this._nameInput.value.trim(),
                surname: this._surnameInput.value.trim(),
                email: this._emailInput.value.trim(),
            };

            ajax({
                url: "http://localhost:9000/proceed",
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: payload,
            }).subscribe({
                next: res => console.log(res.response),
                error: err => console.error(err),
            });
        });
    }
}