import { ajax } from "rxjs/ajax";
import { fromEvent } from "rxjs";
import { map } from "rxjs/operators";

function sendData(e) {
    e.preventDefault();
    let formData = new FormData(e.target);
    let obj = {};
    formData.forEach((value, key)=>{obj[key] = value.valueOf()});
    console.log(obj);
    ajax({
        url: "przetworzDane",
        method:"post",
        headers: {
            "Content-Type": "application/json"
        },
        body: obj
    })
    .pipe(map(odp=>odp.response))
    .subscribe(dane=>console.log("Odpowiedź z serwera: ", dane));
};

fromEvent(document.querySelector("form"),"submit")
    .subscribe(sendData);