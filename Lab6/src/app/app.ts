import { Component } from '@angular/core';
import { First } from "./first/first";
@Component({
  selector: 'app-root',
  imports: [First],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
}