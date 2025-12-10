import { Component } from '@angular/core';

@Component({
  standalone: true,
  selector: 'add-page',
  template: `
    <h2>Dodaj stronę</h2>
    <form>
      <label>Tytuł: <input type="text"></label>
      <button type="submit">Zapisz</button>
    </form>
  `
})
export class AddPageComponent {}
