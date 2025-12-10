import { ComponentFixture, TestBed } from '@angular/core/testing';

import { First } from './first';

describe('First', () => {
  let component: First;
  let fixture: ComponentFixture<First>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [First]
    })
    .compileComponents();

    fixture = TestBed.createComponent(First);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
