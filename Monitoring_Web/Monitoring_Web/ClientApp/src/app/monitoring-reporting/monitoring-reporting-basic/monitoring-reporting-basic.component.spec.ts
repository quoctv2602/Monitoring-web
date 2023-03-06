import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoringReportingBasicComponent } from './monitoring-reporting-basic.component';

describe('MonitoringReportingBasicComponent', () => {
  let component: MonitoringReportingBasicComponent;
  let fixture: ComponentFixture<MonitoringReportingBasicComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MonitoringReportingBasicComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoringReportingBasicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
