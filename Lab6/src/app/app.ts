import { Component, signal } from '@angular/core';
import { First } from "./first/first";

@Component({
  imports: [First],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
}
