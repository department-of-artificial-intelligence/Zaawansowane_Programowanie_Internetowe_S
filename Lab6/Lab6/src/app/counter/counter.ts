import { Component } from '@angular/core';

@Component({
  selector: 'app-counter',
  standalone: false,
  styleUrl: './counter.css',
  templateUrl: './counter.html',
})
export class Counter {
  Value: number = 0;

  onChangeValue(diff: number){
    this.Value += diff;
  }
}
