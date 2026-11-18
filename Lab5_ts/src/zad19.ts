// Zadanie 19 - kolorowy div śledzi kursor w obrębie wyznaczonego obszaru
import { fromEvent } from "rxjs";
import { map } from "rxjs/operators";

type Position = { x: number; y: number };

export class Zad19 {
  private area: HTMLElement;
  private box: HTMLElement;

  constructor(areaId: string, boxId: string) {
    this.area = document.querySelector<HTMLElement>(`#${areaId}`)!;
    this.box = document.querySelector<HTMLElement>(`#${boxId}`)!;
  }

  public start(): void {
    fromEvent<MouseEvent>(this.area, "mousemove")
      .pipe(map(e => this.toAreaPosition(e)))
      .subscribe(pos => this.moveBox(pos));
  }

  // współrzędne kursora względem lewego górnego rogu obszaru
  private toAreaPosition(e: MouseEvent): Position {
    const rect = this.area.getBoundingClientRect();
    return { x: e.clientX - rect.left, y: e.clientY - rect.top };
  }

  private moveBox(pos: Position): void {
    this.box.style.left = `${pos.x}px`;
    this.box.style.top = `${pos.y}px`;
  }
}