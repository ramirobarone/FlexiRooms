import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ControlRoomComponent } from './control-room.component';

describe('ControlRoomComponent', () => {
  let component: ControlRoomComponent;
  let fixture: ComponentFixture<ControlRoomComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ControlRoomComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ControlRoomComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
