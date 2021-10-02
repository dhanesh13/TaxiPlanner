import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageBookingComponent } from './managebooking.component';

describe('ManagebookingComponent', () => {
  let component: ManageBookingComponent;
  let fixture: ComponentFixture<ManageBookingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManageBookingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageBookingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
