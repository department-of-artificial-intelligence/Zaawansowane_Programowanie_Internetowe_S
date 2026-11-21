import { ajax } from "rxjs/ajax";
import { Component } from "./component";
import { retry, tap, timer } from "rxjs";

export class GetError extends Component{
    constructor(){
        super();       
    }

    public oneError(){
        ajax("http://localhost:9000/error")
            .subscribe({
                next: res => console.log(`How ? ${res.response}`),
                error: err => console.error(`Got error: ${err}`)
            });
    }

    public repeatError(){
        let attempt = 0;

        ajax("http://localhost:9000/error1")
            .pipe(
                tap(() => {
                    attempt += 1;
                    console.log(`Attempt #${attempt} succeeded`);
                }),
                retry({
                    count: 3,
                    delay: (err, retryCount) => {
                        console.warn(`Attempt #${retryCount} failed, retrying in 500ms...`, err);
                        return timer(500);
                    },
                })
            )
            .subscribe({
                next: res => console.log(`Got response: ${res.response}`),
                error: err => console.error(`Final error: ${err}`)
            });
    }
}