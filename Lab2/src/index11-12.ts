import { App } from './Zadanie12/zadanie12';

var app = new App(document.querySelector("button:nth-of-type(1)"), 
                  document.querySelector("button:nth-of-type(2)"),
                  document.querySelector("button:nth-of-type(3)"),
                  document.querySelector("input:nth-of-type(1)"),
                  document.querySelector("input:nth-of-type(2)")
                 );
app.init();
