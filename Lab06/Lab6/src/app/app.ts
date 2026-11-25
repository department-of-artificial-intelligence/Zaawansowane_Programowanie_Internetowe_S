import { Component } from '@angular/core';
import { First } from "./first/first";
import { Example } from './example/example';
@Component({
  selector: 'app-root',
  imports: [First, Example],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
}
