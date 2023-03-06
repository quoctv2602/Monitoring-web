import {
  Component,
  ElementRef,
  EventEmitter,
  Inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { Chart } from 'chart.js';
import ChartDataLabels, { Context } from 'chartjs-plugin-datalabels';
import annotationPlugin from 'chartjs-plugin-annotation';
import { DOCUMENT } from '@angular/common';
import { IDashboardTransactionModel } from 'src/app/_model/IDashboardTransactionModel';
import { IDashboardTransaction_TableModel } from 'src/app/_model/IDashboardTransaction-TableModel';
import { NodeSettingsService } from 'src/app/_service/node-settings.service';
import { EnvironmentModel } from 'src/app/_model/node-settings-model';
import { SignalrService } from 'src/app/_service/signalr.service';
import { IDashboardTrasaction_PendingGraphModel } from 'src/app/_model/IDashboardTransaction-PendingGraphModel';
import { DashboardService } from 'src/app/_service/dashboard.service';
import { NodeType } from 'src/app/_common/_enum';
import { SysEnvironment } from 'src/app/_model/sys-environment';
@Component({
  selector: 'app-transaction-based',
  templateUrl: './transaction-based.component.html',
  styleUrls: ['./transaction-based.component.css'],
})
export class TransactionBasedComponent implements OnInit, OnChanges, OnDestroy {
  @Input()
  isStartSlideShow!: boolean;

  @Input()
  secondMax!: number;

  @Output()
  public onStartGetData = new EventEmitter<{
    isStart: boolean;
    filterValue: { environmentID: number; cipFlow: string; lastest: string };
  }>();

  @Input()
  transactionData!: IDashboardTransactionModel;

  @ViewChild('barChart') barCanvas!: ElementRef;
  barChart!: any;

  EnvironmentList: SysEnvironment[] = [];

  selectedEnvironment!: SysEnvironment;

  cipFlowList: any = [
    { value: 'I', name: 'Inbound' },
    { value: 'O', name: 'Outbound' },
  ];

  selectedCIPFlow = { value: 'I', name: 'Inbound' };

  LatestList: any = [
    { ID: 'Today', Name: 'Today' },
    { ID: 'This Month', Name: 'This Month', disable: true },
    { ID: 'This Year', Name: 'This Year', disable: true },
  ];

  LatestSelect = { ID: 'Today', Name: 'Today' };

  TransactionList: IDashboardTransaction_TableModel[] = [];

  ActiveslideShow: boolean = false;

  second = 0;

  indexSelectedEnvironment: number = 0;

  indexSelectedCIPFlow: number = 0;

  pendingGraphData: IDashboardTrasaction_PendingGraphModel[] = [];

  public LineChart: any;

  public BarChart: any;

  myInterval: any;

  constructor(
    @Inject(DOCUMENT) private document: any,
    private nodeSettingService: NodeSettingsService,
    private signalRService: SignalrService,
    private dashboardService: DashboardService
  ) {
    Chart.register(annotationPlugin);
  }

  ngOnInit(): void {
    this.startTimer();
    this.createBarChart([]);
    this.createLineChart([], [], 0);
    this.dashboardService
      .getEnvironmentsListByNodeType(NodeType.TransactionBased)
      .subscribe(
        (res) => {
          this.EnvironmentList = res;
          this.selectedEnvironment = this.EnvironmentList[0];
        },
        (err) => {},
        () => {
          this.signalRService.subjectCompletedGetConnectionId.subscribe(
            (isCompleted) => {
              if (isCompleted == true) this.handleGetData();
            }
          );
        }
      );
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      changes.secondMax &&
      changes.secondMax.currentValue !== null &&
      changes.secondMax.currentValue !== undefined
    ) {
      if (this.ActiveslideShow !== this.isStartSlideShow)
        this.ChangeActiveSlideShow();
    }
    if (
      changes.isStartSlideShow &&
      changes.isStartSlideShow.currentValue !== null &&
      changes.isStartSlideShow.currentValue !== undefined
    ) {
      if (this.ActiveslideShow !== changes.isStartSlideShow.currentValue)
        this.ChangeActiveSlideShow();
    }
    if (changes.transactionData && changes.transactionData.currentValue) {
      const responeData = changes.transactionData
        .currentValue as IDashboardTransactionModel;
      this.onUpdateData(responeData);
    }
  }

  ngOnDestroy(): void {
    clearInterval(this.myInterval);
  }

  resetLineChart() {
    if (this.LineChart) {
      this.LineChart.data.labels = [];
      this.LineChart.data.datasets = [];
      this.LineChart.options.plugins.annotation.annotations = [];
      this.LineChart.update();
    }
    this.pendingGraphData = [];
  }

  onUpdateData(transactionData: IDashboardTransactionModel) {
    this.TransactionList = transactionData.tableData;
    const formatDataColumnChart = this.transactionData.columnChartData
      .slice()
      .map((item) => {
        const color =
          item.errorStatus.trim().toLowerCase() === 'error'
            ? '#D33F48'
            : '#f6b818';
        return {
          label: item.errorStatus,
          data: [item.numberOfTransactions ?? 0],
          backgroundColor: color,
          borderColor: color,
          borderWidth: 1,
        };
      });

    this.createBarChart(formatDataColumnChart);
    const thresholdPendingGraph = transactionData.thresholdPendingGraph;
    if (
      this.pendingGraphData.length === 0 &&
      (transactionData.pendingGraphData.length > 0 || thresholdPendingGraph)
    ) {
      this.pendingGraphData = transactionData.pendingGraphData;
      const labelPendingGraph = this.pendingGraphData
        .slice()
        .filter((a) => a.data >= 0)
        .map((item) => item.label);
      const dataPendingGraph = this.pendingGraphData
        .slice()
        .filter((a) => a.data >= 0)
        .map((item) => item.data);
      this.createLineChart(
        labelPendingGraph,
        dataPendingGraph,
        thresholdPendingGraph ?? 0
      );
    } else if (this.pendingGraphData.length > 0) {
      this.pendingGraphData = transactionData.pendingGraphData;
      this.onRefreshPendingGraph(
        transactionData.pendingGraphData,
        thresholdPendingGraph
      );
    }
  }

  onRefreshPendingGraph(
    pendingGraphData: IDashboardTrasaction_PendingGraphModel[],
    threshold: number | null
  ) {
    if (pendingGraphData.length > 0) {
      const newLabels = pendingGraphData.slice().map((item) => item.label);
      const newDataSet = pendingGraphData.slice().map((item) => item.data);
      const oldDatas = this.LineChart?.data;
      const oldLabels = oldDatas.labels as string[];
      const oldDataSets = oldDatas?.datasets?.[0]?.data;
      if (oldLabels.length > 0) {
        const lastestOldLabel = oldLabels[oldLabels.length - 1];
        const lastestNewLabel = newLabels[newLabels.length - 1];
        const lastestOldData = oldDataSets[oldDataSets.length - 1];
        const lastestNewData = newDataSet[newDataSet.length - 1];
        if (lastestNewLabel !== lastestOldLabel) {
          oldLabels.splice(0, 1);
          oldLabels.push(lastestNewLabel);
          oldDataSets.splice(0, 1);
          oldDataSets.push(lastestNewData);
          this.LineChart.update();
        }
      }
    } else {
      this.LineChart.data.labels = [];
      this.LineChart.data.datasets = [];
      this.LineChart.options.plugins.annotation.annotations = [];
      this.LineChart.update();
    }
  }

  onRefresh() {
    this.selectedEnvironment = this.EnvironmentList[0];
    this.selectedCIPFlow = this.cipFlowList[0];
    this.LatestSelect = this.LatestList[0];
    const filterValue = {
      environmentID: this.selectedEnvironment.id,
      cipFlow: this.selectedCIPFlow.value,
      lastest: 'today',
    };
    this.onStartGetData.emit({ isStart: true, filterValue });
  }

  startTimer() {
    this.myInterval = setInterval(() => {
      if (this.ActiveslideShow == true) {
        this.second++;
        if (this.second % this.secondMax === 0) {
          this.selectedCIPFlow = this.cipFlowList[++this.indexSelectedCIPFlow];
          if (this.indexSelectedCIPFlow == this.cipFlowList.length) {
            this.indexSelectedCIPFlow = 0;
            this.selectedCIPFlow = this.cipFlowList[0];
            this.selectedEnvironment =
              this.EnvironmentList[++this.indexSelectedEnvironment];
            if (this.indexSelectedEnvironment == this.EnvironmentList.length) {
              this.indexSelectedEnvironment = 0;
              this.selectedEnvironment = this.EnvironmentList[0];
            }
          }
          this.handleGetData();
        }
      }
    }, 1000);
  }

  ChangeActiveSlideShow() {
    if (this.ActiveslideShow == true) {
      this.ActiveslideShow = false;
    } else {
      this.ActiveslideShow = true;
      this.second = 0;
    }
  }

  createBarChart(
    datasets: {
      label: string;
      data: number[];
      backgroundColor: string;
      borderColor: string;
      borderWidth: number;
    }[]
  ) {
    if (this.BarChart) this.BarChart.destroy();
    this.BarChart = new Chart('BarChart', {
      type: 'bar',
      data: {
        labels: [''],
        datasets: datasets,
      },
      plugins: [ChartDataLabels],
      options: {
        plugins: {
          legend: {
            display: false,
            title: {
              display: false,
            },
          },
          datalabels: {
            color: '#fff',
            formatter: function (value, context: Context) {
              if (value > 0) return Math.round(value);
              else return '';
            },
          },
        },
        layout: {
          padding: 20,
        },
        aspectRatio: 2.5,
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          y: {
            grid: { display: false },
          },
          x: {
            grid: { display: false },
            offset: true, // <-- THIS
          },
        },
      },
    });
  }

  createLineChart(labels: string[], data: number[], threshold: number) {
    if (this.LineChart) this.LineChart.destroy();
    this.LineChart = new Chart('LineChart', {
      type: 'line',
      data: {
        labels,
        datasets: [
          {
            label: 'Pending Transactions',
            data,
            borderColor: 'rgb(75, 192, 192)',
            backgroundColor: ['rgb(75, 192, 192)'],
            datalabels: {
              align: 'end',
              anchor: 'start',
            },
          },
        ],
      },
      plugins: [ChartDataLabels],
      options: {
        animations: {
          radius: {
            duration: 400,
            easing: 'linear',
            loop: (context) => context.active,
          },
        },
        plugins: {
          annotation: {
            annotations: [
              {
                type: 'line',
                borderColor: 'red',
                borderWidth: 2,
                label: {
                  backgroundColor: 'red',
                  content: 'Threshold: ' + threshold,
                  display: true,
                  position: 'end',
                },
                scaleID: 'y',
                value: threshold,
              },
            ],
          },
          datalabels: {
            // backgroundColor: 'white',
            borderWidth: 2,
            borderRadius: 25,
            color: 'rgb(75, 192, 192)',
            font: {
              weight: 'bold',
            },
            offset: 8,
            padding: 6,
          },
        },
        aspectRatio: 5 / 3,
        layout: {
          padding: {
            top: 32,
            right: 16,
            bottom: 16,
            left: 8,
          },
        },

        elements: {
          line: {
            fill: false,
            tension: 0.4,
          },
        },
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          y: {
            max: 100,
            min: 0,
            ticks: {
              stepSize: 10,
            },
            grid: { display: false },
          },
          x: {
            grid: { display: false },
            //offset: true, // <-- THIS
          },
        },
      },
    });
  }

  handleGetData() {
    this.resetLineChart();
    const filterValue = {
      environmentID: this.selectedEnvironment.id,
      cipFlow: this.selectedCIPFlow.value,
      lastest: 'today',
    };
    this.onStartGetData.emit({ isStart: true, filterValue });
  }

  getClass(value: string | null) {
    if (value === 'IntegrationError' || value === 'Integration Error') {
      return 'badge bg-warning';
    } else {
      return 'badge bg-danger';
    }
  }

  getTotalSummaryTransaction() {
    return this.transactionData.columnChartData.reduce(
      (acc, current) => acc + (current.numberOfTransactions ?? 0),
      0
    );
  }

  onChangeEnvironment($event: EnvironmentModel) {
    this.resetLineChart();
    const filterValue = {
      environmentID: $event.id,
      cipFlow: this.selectedCIPFlow.value,
      lastest: 'today',
    };
    this.onStartGetData.emit({ isStart: true, filterValue });
  }

  onChangeCIPFlow($event: { value: string; name: string }) {
    this.resetLineChart();
    const filterValue = {
      environmentID: this.selectedEnvironment.id,
      cipFlow: $event.value,
      lastest: 'today',
    };
    this.onStartGetData.emit({ isStart: true, filterValue });
  }
}
