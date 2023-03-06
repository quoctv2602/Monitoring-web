import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoringReportingAdvanceComponent } from './monitoring-reporting-advance.component';

describe('MonitoringReportingAdvanceComponent', () => {
  let component: MonitoringReportingAdvanceComponent;
  let fixture: ComponentFixture<MonitoringReportingAdvanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MonitoringReportingAdvanceComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoringReportingAdvanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
