/*Start #F04-Khidir*/
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddotherbookingsComponent } from './addotherbookings.component';

describe('AddotherbookingsComponent', () => {
  let component: AddotherbookingsComponent;
  let fixture: ComponentFixture<AddotherbookingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddotherbookingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddotherbookingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
/*Start #F04-Khidir*/