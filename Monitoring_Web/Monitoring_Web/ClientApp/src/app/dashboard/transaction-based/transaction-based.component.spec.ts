import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionBasedComponent } from './transaction-based.component';

describe('TransactionBasedComponent', () => {
  let component: TransactionBasedComponent;
  let fixture: ComponentFixture<TransactionBasedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TransactionBasedComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransactionBasedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
