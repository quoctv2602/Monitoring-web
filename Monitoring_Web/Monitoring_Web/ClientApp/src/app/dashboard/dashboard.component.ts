import {
  AfterViewInit,
  Component,
  ElementRef,
  Inject,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  Chart,
  registerables,
  ChartDataset,
  ChartConfiguration,
  ChartData,
} from 'chart.js';

import { map, Subscription } from 'rxjs';
import { SignalrService } from '../_service/signalr.service';
import { DashboardService } from '../_service/dashboard.service';
import { NodeSettingsService } from '../_service/node-settings.service';
import { INodeSettingDashboardHealthModel } from '../_model/INodeSettingDashboardHealthModel';
import { IDashboardSystemHealthModel } from '../_model/IDashboardSystemHealthModel';
import { IServiceModel } from '../_model/IServiceModel';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import annotationPlugin from 'chartjs-plugin-annotation';
import { AnnotationOptions } from 'chartjs-plugin-annotation';
import { IDashboardSystemHealthByFreeDiskModel } from '../_model/IDashboardSystemHealthByFreeDiskModel';
import {
  ActionEnum,
  DashboardType,
  MonitoringType,
  NodeType,
} from '../_common/_enum';
import { SysMonitoring } from '../_model/sys-monitoring';
import { DOCUMENT } from '@angular/common';
import { IServiceListRequestModel } from '../_model/IServiceListRequestModel';
import { IDashboardRequest } from '../_model/IDashboardRequest';
import { IDashboardTransactionRequest } from '../_model/IDashboardTransactionRequest';
import { IDashboardTransactionModel } from '../_model/IDashboardTransactionModel';
import { BaseComponent } from '../base.component';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent
  extends BaseComponent
  implements OnInit, AfterViewInit, OnDestroy
{
  lineChart!: any;

  freeDiskChart!: any;

  subscriptionReceiveData!: Subscription;

  monitoringType: number = 1;

  listNodes: INodeSettingDashboardHealthModel[] = [];

  dataServiceWithStatus: IServiceModel[] = [];

  dataServiceWithInstanceCounter: IServiceModel[] = [];

  isSelectAll: boolean = false;

  isLoading: boolean = false;

  isLoadingChart: boolean = false;

  showButtonService: boolean = false;

  isLoadingServices: boolean = false;

  isFirstTimeLoad!: boolean | null;

  isSelectOrUnselectNode!: boolean;

  intervalSlideKPI!: number;

  currentIndexKPI: number = 0;

  isStartSlideShow: boolean = false;

  myInterval!: any;

  elem: any;

  selectedNodeName: string = '';

  @ViewChild('baseChart') lineCanvas!: ElementRef;

  @ViewChild('freeDisk') freeDisk!: ElementRef;

  showFreeDisk!: boolean;

  currentUnitChart: string = '%';

  skipped = (ctx: any, value: any) =>
    ctx.p0.skip || ctx.p1.skip ? value : undefined;

  down = (ctx: any, value: any) =>
    ctx.p0.parsed.y > ctx.p1.parsed.y ? value : undefined;

  colorsArray = [
    'rgba(99,181,152)',
    'rgba(206,125,120)',
    'rgba(234,158,112)',
    'rgba(164,138,158)',
    'rgba(198,225,232)',
    'rgba(100,129,119)',
    'rgba(13,90,193)',
    'rgba(242,5,230)',
    'rgba(28,3,101)',
    'rgba(20,169,173)',
    'rgba(76,162,249)',
    'rgba(164,228,63)',
    'rgba(210,152,226)',
    'rgba(97,25,208)',
    'rgba(210,115,125)',
    'rgba(192,164,60)',
    'rgba(242,81,14)',
    'rgba(101,27,230)',
    'rgba(121,128,110)',
    'rgba(97,218,94)',
    'rgba(205,47,0)',
    'rgba(147,72,175)',
    'rgba(1,172,83)',
    'rgba(197,164,251)',
    'rgba(153,102,53)',
    'rgba(177,21,115)',
    'rgba(75,180,115)',
    'rgba(117,216,158)',
    'rgba(47,63,148)',
    'rgba(47,123,153)',
    'rgba(218,150,125)',
    'rgba(52,137,31)',
    'rgba(176,216,123)',
    'rgba(202,71,81)',
    'rgba(126,80,168)',
    'rgba(196,214,71)',
    'rgba(224,238,184)',
    'rgba(17,222,193)',
    'rgba(40,152,18)',
    'rgba(86,108,160)',
    'rgba(255,219,225)',
    'rgba(47,17,121)',
    'rgba(147,91,109)',
    'rgba(145,105,136)',
    'rgba(81,61,152)',
    'rgba(174,173,58)',
    'rgba(158,109,113)',
    'rgba(75,91,220)',
    'rgba(12,211,109)',
    'rgba(37,6,98)',
    'rgba(203,91,234)',
    'rgba(34,137,22)',
    'rgba(172,62,27)',
    'rgba(223,81,74)',
    'rgba(83,147,151)',
    'rgba(136,9,119)',
    'rgba(246,151,193)',
    'rgba(186,150,206)',
    'rgba(103,156,157)',
    'rgba(198,196,44)',
    'rgba(93,44,82)',
    'rgba(72,180,27)',
    'rgba(225,207,59)',
    'rgba(91,228,240)',
    'rgba(87,196,216)',
    'rgba(164,209,122)',
    'rgba(190,96,139)',
    'rgba(150,176,12)',
    'rgba(8,139,175)',
    'rgba(241,88,191)',
    'rgba(225,69,186)',
    'rgba(238,145,227)',
    'rgba(5,211,113)',
    'rgba(84,38,224)',
    'rgba(72,52,208)',
    'rgba(128,34,52)',
    'rgba(103,73,232)',
    'rgba(9,113,240)',
    'rgba(143,180,19)',
    'rgba(178,180,240)',
    'rgba(195,200,157)',
    'rgba(201,169,65)',
    'rgba(65,209,88)',
    'rgba(251,33,163)',
    'rgba(81,174,217)',
    'rgba(91,179,45)',
    'rgba(33,83,142)',
    'rgba(137,213,52)',
    'rgba(211,102,71)',
    'rgba(127,180,17)',
    'rgba(0,35,184)',
    'rgba(59,140,42)',
    'rgba(152,107,83)',
    'rgba(245,4,34)',
    'rgba(152,63,122)',
    'rgba(234,36,163)',
    'rgba(121,53,44)',
    'rgba(82,18,80)',
    'rgba(199,158,210)',
    'rgba(214,221,146)',
    'rgba(227,62,82)',
    'rgba(178,190,87)',
    'rgba(250,6,236)',
    'rgba(27,182,153)',
    'rgba(107,46,95)',
    'rgba(100,130,15)',
    'rgba(33,83,142)',
    'rgba(137,213,52)',
    'rgba(211,102,71)',
    'rgba(127,180,17)',
    'rgba(0,35,184)',
    'rgba(59,140,42)',
    'rgba(152,107,83)',
    'rgba(245,4,34)',
    'rgba(152,63,122)',
    'rgba(234,36,163)',
    'rgba(121,53,44)',
    'rgba(82,18,80)',
    'rgba(199,158,210)',
    'rgba(214,221,146)',
    'rgba(227,62,82)',
    'rgba(178,190,87)',
    'rgba(250,6,236)',
    'rgba(27,182,153)',
    'rgba(107,46,95)',
    'rgba(100,130,15)',
    'rgba(156,182,74)',
    'rgba(153,108,72)',
    'rgba(154,185,183)',
    'rgba(6,224,82)',
    'rgba(227,164,129)',
    'rgba(14,182,33)',
    'rgba(252,69,142)',
    'rgba(178,219,21)',
    'rgba(170,34,109)',
    'rgba(121,46,216)',
    'rgba(115,135,42)',
    'rgba(82,13,58)',
    'rgba(206,252,184)',
    'rgba(165,179,217)',
    'rgba(125,29,133)',
    'rgba(196,253,87)',
    'rgba(241,174,22)',
    'rgba(143,226,42)',
    'rgba(239,110,60)',
    'rgba(36,62,235)',
    'rgba(221,147,253)',
    'rgba(63,132,115)',
    'rgba(231,219,206)',
    'rgba(66,31,121)',
    'rgba(122,61,147)',
    'rgba(99,95,109)',
    'rgba(147,242,215)',
    'rgba(155,92,42)',
    'rgba(21,185,238)',
    'rgba(15,89,151)',
    'rgba(64,145,136)',
    'rgba(145,30,32)',
    'rgba(19,80,206)',
    'rgba(16,229,177)',
    'rgba(255,244,215)',
    'rgba(203,37,130)',
    'rgba(206,0,190)',
    'rgba(50,213,214)',
    'rgba(96,133,114)',
    'rgba(199,155,194)',
    'rgba(0,248,124)',
    'rgba(119,119,42)',
    'rgba(105,149,186)',
    'rgba(252,107,87)',
    'rgba(240,120,21)',
    'rgba(143,216,131)',
    'rgba(6,14,39)',
    'rgba(150,229,145)',
    'rgba(33,213,46)',
    'rgba(208,0,67)',
    'rgba(180,113,98)',
    'rgba(30,194,39)',
    'rgba(79,15,111)',
    'rgba(29,29,88)',
    'rgba(148,112,2)',
    'rgba(189,224,82)',
    'rgba(224,140,86)',
    'rgba(40,252,253)',
    'rgba(54,72,106)',
    'rgba(208,46,41)',
    'rgba(26,230,219)',
    'rgba(62,70,76)',
    'rgba(168,74,143)',
    'rgba(145,30,126)',
    'rgba(63,22,217)',
    'rgba(15,82,95)',
    'rgba(172,124,10)',
    'rgba(180,192,134)',
    'rgba(201,215,48)',
    'rgba(48,204,73)',
    'rgba(61,103,81)',
    'rgba(251,76,3)',
    'rgba(100,15,193)',
    'rgba(98,192,62)',
    'rgba(211,73,58)',
    'rgba(136,170,11)',
    'rgba(64,109,249)',
    'rgba(97,90,240)',
    'rgba(42,52,52)',
    'rgba(74,84,63)',
    'rgba(121,188,160)',
    'rgba(168,184,212)',
    'rgba(0,239,212)',
    'rgba(122,210,54)',
    'rgba(114,96,216)',
    'rgba(29,234,167)',
    'rgba(6,244,58)',
    'rgba(130,60,89)',
    'rgba(227,217,76)',
    'rgba(220,28,6)',
    'rgba(245,59,42)',
    'rgba(180,98,56)',
    'rgba(45,255,246)',
    'rgba(168,43,137)',
    'rgba(26,128,17)',
    'rgba(67,106,159)',
    'rgba(26,128,106)',
    'rgba(76,240,157)',
    'rgba(193,136,162)',
    'rgba(103,235,75)',
    'rgba(179,8,211)',
    'rgba(252,126,65)',
    'rgba(175,49,1)',
    'rgba(113,177,244)',
    'rgba(162,248,165)',
    'rgba(226,61,208)',
    'rgba(211,72,109)',
    'rgba(0,247,249)',
    'rgba(71,72,147)',
    'rgba(60,236,53)',
    'rgba(28,101,203)',
    'rgba(93,29,12)',
    'rgba(45,125,42)',
    'rgba(255,52,32)',
    'rgba(92,221,135)',
    'rgba(162,89,164)',
    'rgba(228,172,68)',
    'rgba(27,237,230)',
    'rgba(135,152,164)',
    'rgba(215,121,15)',
    'rgba(178,194,79)',
    'rgba(222,115,194)',
    'rgba(215,10,156)',
    'rgba(136,233,184)',
    'rgba(194,176,226)',
    'rgba(134,233,143)',
    'rgba(174,144,226)',
    'rgba(26,128,107)',
    'rgba(67,106,158)',
    'rgba(14,192,255)',
    'rgba(248,18,179)',
    'rgba(177,127,201)',
    'rgba(141,108,47)',
    'rgba(211,39,122)',
    'rgba(44,161,174)',
    'rgba(150,133,235)',
    'rgba(138,150,198)',
    'rgba(219,162,230)',
    'rgba(118,252,27)',
    'rgba(96,143,164)',
    'rgba(32,246,186)',
    'rgba(7,215,246)',
    'rgba(220,231,122)',
    'rgba(119,236,202)',
  ];

  listMonitoringType: SysMonitoring[] = [];

  thresholdFreeDisk: {
    threshold: number;
    color: string;
    machineName: string;
    labelClass: string;
    environmentID: number;
  }[] = [];

  showCustomLegend: boolean = false;

  lineFreeDiskInfo: {
    type: string;
    borderColor: string;
    borderWidth: number;
    scaleID: string;
    value: number;
    machineName: string;
    environmentID: number;
    active: boolean;
  }[] = [];

  dashboardType: number = -1;

  transactionData!: IDashboardTransactionModel;

  inItSystemhealth:boolean=false

  constructor(
    private signalRService: SignalrService,
    private _dashboardService: DashboardService,
    private _nodeSettingService: NodeSettingsService,
    @Inject(DOCUMENT) private _document: any
  ) {
    super();
    Chart.register(...registerables);
    Chart.register(annotationPlugin);
  }

  ngOnDestroy(): void {
    this.inItSystemhealth=false;
    this.signalRService.closeConnection();
  }

  ngOnInit(): void {
    super.ngOnInit();
    if (this.roles.indexOf(ActionEnum.dashboardViewTransactionBased) !== -1)
      this.dashboardType = DashboardType.TransactionBased;
    else if (this.roles.indexOf(ActionEnum.dashboardViewSystemBased) !== -1)
      this.dashboardType = DashboardType.SystemHealth;
    this._nodeSettingService.getSysMonitoring().subscribe((res) => {
      this.listMonitoringType = res
        .slice()
        .filter((a) => a.nodeType === NodeType.SystemHealth);
      this.selectedNodeName = this.listMonitoringType[0].name;
    },err=>{},
    ()=>{
      if(this.dashboardType===DashboardType.SystemHealth)
        this.onClickGetNode()
    });
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();
    this.signalRService.subjectReceiveData.subscribe((res) => {
      if (res !== null && res !== undefined) {
        if (this.monitoringType === MonitoringType.FreeDisk)
          this.onRefreshFreeDisk(res as any);
        else this.onRefreshUpdate(res);
      }
    });
    this.signalRService.subjectReceiveTransactionData.subscribe((res) => {
      if (res !== null && res !== undefined) {
        this.transactionData = res;
      }
    });
  }

  ngAfterViewInit(): void {
    this.drawChartLine([], [], []);
    this.drawChartFreeDisk([], [], []);
  }

  onRefresh = (
    dataRefresh: {
      environmentID: number;
      environmentName: string;
      machineName: string;
      data: number;
      label: string;
      threshold: number;
      requestID: string;
      nodeID: number;
      dateString: string;
    }[]
  ) => {
    try {
      this.thresholdFreeDisk = [];
      this.lineFreeDiskInfo = [];
      if (dataRefresh && dataRefresh.length > 0) {
        const unitChart = this.listMonitoringType.find(
          (a) => a.id === this.monitoringType
        )?.unit;
        this.currentUnitChart = unitChart as string;
        const arrayDateAndTime = dataRefresh.slice().map((item) => {
          return { dateString: item.dateString, time: item.label };
        });
        arrayDateAndTime.sort((a, b) => {
          var dateInfoA: any = a.dateString.split('-').map((item) => {
            return Number.parseInt(item);
          });
          var dateInfoB: any = b.dateString.split('-').map((item) => {
            return Number.parseInt(item);
          });
          var timeInfoA: any = a.time.split(':').map((item) => {
            return Number.parseInt(item);
          });
          var timeInfoB: any = b.time.split(':').map((item) => {
            return Number.parseInt(item);
          });
          const dateA = new Date(
            dateInfoA[0],
            dateInfoA[1],
            dateInfoA[2],
            timeInfoA[0],
            timeInfoA[1],
            timeInfoA[2]
          );
          const dateB = new Date(
            dateInfoB[0],
            dateInfoB[1],
            dateInfoB[2],
            timeInfoB[0],
            timeInfoB[1],
            timeInfoB[2]
          );
          return dateA.getTime() - dateB.getTime();
        });
        const uniqueTime = [
          ...new Set(arrayDateAndTime.map((item) => item.time)),
        ]; // [ 'A', 'B']

        const listSelectingNode = this.listNodes.filter(
          (i) => i.selecting === true
        );
        let dataSet: ChartDataset<'line', { x: string; y: number }[]>[] = [];
        let dataSetThreshold: ChartDataset<
          'line',
          { x: string; y: number }[]
        >[] = [];
        let thresholds: any[] = [];
        for (let j = 0; j < listSelectingNode.length; j++) {
          const nodeInfo = listSelectingNode[j];

          const label = listSelectingNode[j].nodeName;
          let data = [];
          let dataThreshold = [];
          let lastestValue: number = -1;
          let lastestRequest: string = '';
          let dataByNode = dataRefresh.filter(
            (a) =>
              a.environmentID === nodeInfo.environmentID &&
              a.machineName === nodeInfo.nodeName
          );
          let thresholdByNode = 0;
          if (dataByNode.length > 0) {
            thresholdByNode = dataByNode[0].threshold;
          }

          for (let i = 0; i < uniqueTime.length; i++) {
            let time = uniqueTime[i];
            let threshold = 0;
            let dataByTime = dataByNode.find((db) => db.label === time);
            if (dataByTime) {
              if (dataByTime.data > 0) {
                lastestValue = dataByTime.data;
                lastestRequest = dataByTime.requestID;
                data.push({
                  x: time,
                  y: dataByTime && dataByTime.data ? dataByTime.data : NaN,
                });
              }
              if (threshold === 0 && dataByTime.threshold)
                threshold = dataByTime.threshold;
            }
            dataThreshold.push({
              x: time,
              y: threshold,
            });
          }
          const thresholdMasterData = dataThreshold.find((a) => a.y > 0)?.y;
          let threshold = thresholdMasterData ? thresholdMasterData : 0;
          const clonedataThreshold = [...dataThreshold].map((item) => {
            return {
              ...item,
              y: thresholdMasterData ? thresholdMasterData : item.y,
            };
          });
          const color = this.colorsArray[j];
          const thresholdColor = color.slice(0, -1) + ',0.4)';
          const itemThreshold = {
            type: 'line',
            borderColor: thresholdColor,
            borderWidth: 2,
            scaleID: 'y',
            value: thresholdByNode,
            machineName: nodeInfo.nodeName,
            environmentID: nodeInfo.environmentID,
          };
          thresholds.push(itemThreshold);
          this.thresholdFreeDisk.push({
            machineName: nodeInfo.nodeName,
            color: thresholdColor,
            threshold: thresholdByNode,
            labelClass: 'label-active',
            environmentID: nodeInfo.environmentID,
          });
          if (data.findIndex((a) => a.y > 0) !== -1) {
            dataSet.push({
              label,
              data,
              backgroundColor: color,
              borderColor: color,
              pointBorderColor: color,
              pointBackgroundColor: color,
              pointStyle: 'dash',
              pointBorderWidth: 0,
              borderWidth: 2,
              segment: {
                borderDash: (ctx) => this.skipped(ctx, [6, 6]),
              },
              spanGaps: true,
            });
          }
          // dataSetThreshold.push({
          //   label: label + ': Threshold ' + threshold + ' ' + unitChart,
          //   data: clonedataThreshold,
          //   backgroundColor: thresholdColor,
          //   borderColor: thresholdColor,
          //   pointBorderColor: thresholdColor,
          //   pointBackgroundColor: thresholdColor,
          //   pointStyle: 'dash',
          //   pointBorderWidth: 0,
          //   borderWidth: 3,
          // });
          let dataNodes = this.listNodes.find(
            (ds) =>
              ds.environmentID === nodeInfo.environmentID &&
              ds.nodeName === nodeInfo.nodeName
          );
          if (dataNodes) {
            if (lastestValue >= 0) {
              dataNodes.utilization = lastestValue + ' ' + unitChart;
              dataNodes.requestID = lastestRequest;
            } else {
              dataNodes.utilization = '';
            }
          }
        }
        // for (let i = 0; i < dataSetThreshold.length; i++) {
        //   dataSet.push(dataSetThreshold[i]);
        // }
        this.showCustomLegend = true;
        this.lineFreeDiskInfo = (thresholds as any[]).slice().map((item) => {
          return { ...item, active: true };
        });
        if (dataSet.length > 0) {
          this.lineChart.data.labels = uniqueTime;
          this.lineChart.data.datasets = dataSet;
        } else {
          this.lineChart.data.labels = [];
          this.lineChart.data.datasets = [];
        }
        this.lineChart.options.scales.y.title.text = unitChart;
        this.lineChart.options.plugins.annotation.annotations = thresholds;
        if (unitChart === '%') this.lineChart.options.scales.y.max = 100;
        else delete this.lineChart.options.scales.y.max;
        this.lineChart.update();
        this.isLoadingChart = false;
      } else {
        if (this.lineChart) {
          this.lineChart.data.labels = [];
          this.lineChart.data.datasets = [];
          this.lineChart.options.plugins.annotation.annotations = [];
          this.lineChart.update();
          this.isLoadingChart = false;
          this;
        }
      }
    } catch (e) {
      this.isLoadingChart = false;
    }
  };

  onRefreshFreeDisk(dataRefresh: IDashboardSystemHealthByFreeDiskModel[]) {
    this.thresholdFreeDisk = [];
    this.lineFreeDiskInfo = [];
    let indexColor = 0;
    let distinctNode: {
      machineName: string;
      environmentID: number;
      color: string;
      threshold: number;
      requestID?: string;
    }[] = [];
    let distinctDriveName: string[] = [];

    dataRefresh.filter((item) => {
      var i = distinctNode.findIndex(
        (x) =>
          x.machineName == item.machineName &&
          x.environmentID == item.environmentID
      );
      if (i <= -1) {
        const color = this.colorsArray[indexColor++];
        distinctNode.push({
          machineName: item.machineName,
          environmentID: item.environmentID,
          color: color,
          threshold: item.threshold,
          requestID: item.requestID,
        });
        this.thresholdFreeDisk.push({
          machineName: item.machineName,
          color: color,
          threshold: item.threshold,
          labelClass: 'label-active',
          environmentID: item.environmentID,
        });
      }
      return null;
    });
    dataRefresh.filter(function (item) {
      var i = distinctDriveName.findIndex((x) => x == item.driveName);
      if (i <= -1) {
        distinctDriveName.push(item.driveName);
      }
      return null;
    });
    this.freeDiskChart.data.labels = distinctNode
      .slice()
      .map((item) => item.machineName);
    let dataSets: {
      driveName: string;
      label: string;
      data: number[];
      backgroundColor: string[];
      stack: string;
    }[] = [];
    distinctDriveName.slice().map((driveName, index) => {
      const label = driveName;
      let data: number[] = [];
      let dataFree: number[] = [];
      let backgroundColor: string[] = [];
      distinctNode.slice().map((item) => {
        const dataByDrive = dataRefresh.find(
          (a) =>
            a.driveName == driveName &&
            a.environmentID == item.environmentID &&
            a.machineName == item.machineName
        );
        backgroundColor.push(item.color);
        data.push(dataByDrive ? dataByDrive.percentUsedSpace : NaN);
        dataFree.push(dataByDrive ? dataByDrive.percentFreeSpace : NaN);
        var nodeInfo = this.listNodes.find(
          (s) =>
            s.environmentID == item.environmentID &&
            s.nodeName == item.machineName
        );
        if (nodeInfo) {
          nodeInfo.requestID = item.requestID ? item.requestID : '';
        }
      });
      dataSets.push({
        driveName,
        label, //: label + ' (Used space)',
        data: dataFree,
        backgroundColor: backgroundColor.slice().map((item) => '#c6c6c6'),
        stack: driveName,
      });
      dataSets.push({
        driveName,
        label, //: label + ' (Free space)',
        data: data,
        backgroundColor: backgroundColor,
        stack: driveName,
      });
    });
    this.freeDiskChart.data.datasets = dataSets;
    let threshold: AnnotationOptions[] = distinctNode.slice().map((item) => {
      return {
        type: 'line',
        borderColor: item.color,
        borderWidth: 2,
        scaleID: 'x',
        value: item.threshold,
        machineName: item.machineName,
        environmentID: item.environmentID,
      };
    });
    this.lineFreeDiskInfo = (threshold as any[]).slice().map((item) => {
      return { ...item, active: true };
    });
    this.freeDiskChart.options.plugins.annotation.annotations = threshold;
    this.freeDiskChart.update();
    this.listNodes = this.listNodes.map((item) => {
      return { ...item, utilization: '' };
    });
    this.isLoadingChart = false;
    this.showCustomLegend = true;
  }

  onRefreshUpdate = (
    dataRefresh: {
      environmentID: number;
      environmentName: string;
      machineName: string;
      data: number;
      label: string;
      threshold: number;
      requestID: string;
      nodeID: number;
      dateString: string;
    }[]
  ) => {
    const unitChart = this.listMonitoringType.find(
      (a) => a.id === this.monitoringType
    )?.unit;
    this.currentUnitChart = unitChart as string;
    if (this.lineChart)
      if (unitChart === '%') this.lineChart.options.scales.y.max = 100;
      else delete this.lineChart.options.scales.y.max;
    if (dataRefresh && dataRefresh.length > 0) {
      if (this.isFirstTimeLoad === undefined || this.isFirstTimeLoad === null)
        this.isFirstTimeLoad = true;
      else this.isFirstTimeLoad = false;
      if (!this.isFirstTimeLoad) {
        const data = this.lineChart.data;
        const labelsChart = data.labels as string[];
        const arrayDateAndTime = dataRefresh.slice().map((item) => {
          return { dateString: item.dateString, time: item.label };
        });
        arrayDateAndTime.sort((a, b) => {
          var dateInfoA: any = a.dateString.split('-').map((item) => {
            return Number.parseInt(item);
          });
          var dateInfoB: any = b.dateString.split('-').map((item) => {
            return Number.parseInt(item);
          });
          var timeInfoA: any = a.time.split(':').map((item) => {
            return Number.parseInt(item);
          });
          var timeInfoB: any = b.time.split(':').map((item) => {
            return Number.parseInt(item);
          });
          const dateA = new Date(
            dateInfoA[0],
            dateInfoA[1],
            dateInfoA[2],
            timeInfoA[0],
            timeInfoA[1],
            timeInfoA[2]
          );
          const dateB = new Date(
            dateInfoB[0],
            dateInfoB[1],
            dateInfoB[2],
            timeInfoB[0],
            timeInfoB[1],
            timeInfoB[2]
          );
          return dateA.getTime() - dateB.getTime();
        });
        const uniqueTime = [
          ...new Set(arrayDateAndTime.map((item) => item.time)),
        ]; // [ 'A', 'B']
        for (let i = 0; i < uniqueTime.length; i++) {
          const timeUpToDate = uniqueTime[i];
          if (!labelsChart.includes(timeUpToDate)) {
            if (data.datasets.length > 0) {
              data.labels.push(timeUpToDate);
              data.labels.splice(data.labels[0], 1);
              for (let index = 0; index < data.datasets.length; ++index) {
                const dataSets = data.datasets[index];
                const dataOfDataSets = dataSets.data;
                if (
                  (dataSets.label as string).toLowerCase().includes('threshold')
                ) {
                  dataOfDataSets.splice(dataOfDataSets[0], 1);
                  dataOfDataSets.push({
                    x: timeUpToDate,
                    y: dataOfDataSets[0].y,
                  });
                } else {
                  let dataByNode = dataRefresh.filter(
                    (a) =>
                      a.machineName === dataSets.label &&
                      a.label === timeUpToDate
                  );
                  if (dataByNode.length > 0) {
                    const environmentID = dataByNode[0].environmentID;
                    const machineName = dataByNode[0].machineName;
                    const lastestValue = dataByNode[0].data + ' ' + unitChart;
                    const lastestRequest = dataByNode[0].requestID;
                    let dataTableNodes = this.listNodes.find(
                      (ds) =>
                        ds.environmentID === environmentID &&
                        ds.nodeName === machineName
                    );
                    if (dataTableNodes) {
                      dataTableNodes.utilization = lastestValue;
                      dataTableNodes.requestID = lastestRequest;
                    }
                  }
                  dataOfDataSets.splice(dataOfDataSets[0], 1);
                  dataOfDataSets.push({
                    x: timeUpToDate,
                    y: dataByNode.length > 0 ? dataByNode[0].data : NaN,
                  });
                }
              }
            }
          }
        }
        this.lineChart.update();
      } else {
        this.thresholdFreeDisk = [];
        this.lineFreeDiskInfo = [];
        const arrayDateAndTime = dataRefresh.slice().map((item) => {
          return { dateString: item.dateString, time: item.label };
        });
        arrayDateAndTime.sort((a, b) => {
          var dateInfoA: any = a.dateString.split('-').map((item) => {
            return Number.parseInt(item);
          });
          var dateInfoB: any = b.dateString.split('-').map((item) => {
            return Number.parseInt(item);
          });
          var timeInfoA: any = a.time.split(':').map((item) => {
            return Number.parseInt(item);
          });
          var timeInfoB: any = b.time.split(':').map((item) => {
            return Number.parseInt(item);
          });
          const dateA = new Date(
            dateInfoA[0],
            dateInfoA[1],
            dateInfoA[2],
            timeInfoA[0],
            timeInfoA[1],
            timeInfoA[2]
          );
          const dateB = new Date(
            dateInfoB[0],
            dateInfoB[1],
            dateInfoB[2],
            timeInfoB[0],
            timeInfoB[1],
            timeInfoB[2]
          );
          return dateA.getTime() - dateB.getTime();
        });
        const uniqueTime = [
          ...new Set(arrayDateAndTime.map((item) => item.time)),
        ]; // [ 'A', 'B']
        const listSelectingNode = this.listNodes.filter(
          (i) => i.selecting === true
        );
        let dataSet: ChartDataset<'line', { x: string; y: number }[]>[] = [];

        let thresholds: any[] = [];

        for (let j = 0; j < listSelectingNode.length; j++) {
          const nodeInfo = listSelectingNode[j];

          const label = listSelectingNode[j].nodeName;
          let data = [];
          let dataThreshold = [];
          let lastestValue: string = '';
          let lastestRequest: string = '';
          let dataByNode = dataRefresh.filter(
            (a) =>
              a.environmentID === nodeInfo.environmentID &&
              a.machineName === nodeInfo.nodeName
          );
          let thresholdByNode = 0;
          if (dataByNode.length > 0) {
            thresholdByNode = dataByNode[0].threshold;
          }

          for (let i = 0; i < uniqueTime.length; i++) {
            let time = uniqueTime[i];
            let threshold = 0;
            let dataByTime = dataByNode.find((db) => db.label === time);
            if (dataByTime) {
              if (dataByTime.data >= 0) {
                lastestValue = dataByTime.data + ' ' + unitChart;
                lastestRequest = dataByTime.requestID;
                data.push({ x: time, y: dataByTime ? dataByTime.data : NaN });
              }
              if (threshold === 0 && dataByTime.threshold)
                threshold = dataByTime.threshold;
            }
            dataThreshold.push({
              x: time,
              y: threshold,
            });
          }
          const color = this.colorsArray[j];
          const thresholdColor = color.slice(0, -1) + ',0.4)';
          const itemThreshold = {
            type: 'line',
            borderColor: thresholdColor,
            borderWidth: 2,
            scaleID: 'y',
            value: thresholdByNode,
            machineName: nodeInfo.nodeName,
            environmentID: nodeInfo.environmentID,
          };
          thresholds.push(itemThreshold);
          this.thresholdFreeDisk.push({
            machineName: nodeInfo.nodeName,
            color: thresholdColor,
            threshold: thresholdByNode,
            labelClass: 'label-active',
            environmentID: nodeInfo.environmentID,
          });
          this.lineFreeDiskInfo = (thresholds as any[]).slice().map((item) => {
            return { ...item, active: true };
          });
          if (data.findIndex((a) => a.y >= 0) !== -1) {
            dataSet.push({
              label,
              data,
              backgroundColor: color,
              borderColor: color,
              pointBorderColor: color,
              pointBackgroundColor: color,
              pointStyle: 'dash',
              pointBorderWidth: 0,
              borderWidth: 2,
              segment: {
                borderDash: (ctx) => this.skipped(ctx, [6, 6]),
              },
              spanGaps: true,
            });
          }
          let dataNodes = this.listNodes.find(
            (ds) =>
              ds.environmentID === nodeInfo.environmentID &&
              ds.nodeName === nodeInfo.nodeName
          );
          if (dataNodes) {
            dataNodes.utilization = lastestValue;
            dataNodes.requestID = lastestRequest;
          }
        }
        if (dataSet.length > 0) {
          this.lineChart.data.labels = uniqueTime;
          this.lineChart.data.datasets = dataSet;
        } else {
          this.lineChart.data.labels = [];
          this.lineChart.data.datasets = [];
        }
        this.lineChart.options.scales.y.title.text = unitChart;
        this.lineChart.options.plugins.annotation.annotations = thresholds;
        this.lineChart.update();
        this.isLoadingChart = false;
        this.showCustomLegend = true;
      }
    } else {
      if (this.lineChart) {
        this.lineChart.data.labels = [];
        this.lineChart.data.datasets = [];
        this.lineChart.options.scales.y.title.text = unitChart;
        this.lineChart.options.plugins.annotation.annotations = [];
        this.lineChart.update();
        this.isLoadingChart = false;
      }
    }
  };

  private startHttpRequest = (requestParam: {
    connectionId: string;
    systemHealthRequest: {
      connectionId: string;
      monitoringType: number;
      nodeSettings: {
        ID: number;
        environmentID: number;
        nodeName: string;
        environmentName: string;
      }[];
    } | null;
    transactionRequest: IDashboardTransactionRequest | null;
  }) => {
    const requestBody: IDashboardRequest = {
      connectionId: requestParam.connectionId,
      dashboardType: this.dashboardType,
      systemHealthRequest: requestParam.systemHealthRequest,
      transactionRequest: requestParam.transactionRequest,
    };
    this._dashboardService.getDataSystemHealth(requestBody).subscribe((res) => {
      const message = res.message;
      if (this.dashboardType === DashboardType.SystemHealth) {
        if (this.monitoringType === MonitoringType.FreeDisk)
          this.onRefreshFreeDisk(res.data);
        else this.onRefreshUpdate(res.data);
      } else {
        this.transactionData = res.data as IDashboardTransactionModel;
      }
    });
  };

  drawChartLine(
    datasets: ChartDataset[],
    labels: string[],
    annotation: AnnotationOptions[]
  ) {
    const ctx = this.lineCanvas.nativeElement.getContext('2d');

    this.lineChart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: labels,
        datasets: datasets,
      },
      options: {
        animations: {
          radius: {
            duration: 400,
            easing: 'linear',
            loop: (context) => context.active,
          },
        },
        interaction: {
          mode: 'nearest',
          intersect: false,
          axis: 'x',
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
            title: {
              display: true,
              text: '%',
            },
          },
          x: {
            grid: { display: false },
          },
        },
        plugins: {
          legend: {
            display: true,
            labels: {
              filter: function (legendItem, data) {
                let label =
                  data.datasets[legendItem.datasetIndex as number].label || '';
                if (typeof label !== 'undefined') {
                  if (label.toLowerCase().includes('threshold')) {
                    return true;
                  }
                }
                return false;
              },
            },
          },
          annotation: {
            annotations: annotation,
          },
          tooltip: {
            callbacks: {
              label: (tooltipItem) => {
                const machineName = tooltipItem.dataset.label;
                const threshold: any = this.thresholdFreeDisk.find(
                  (s) => s.machineName === machineName
                );
                let returnValue: string[] = [
                  tooltipItem.dataset.label +
                    ' : ' +
                    tooltipItem.parsed.y +
                    ' ' +
                    this.currentUnitChart,
                ];
                if (threshold) {
                  returnValue.push(
                    'Threshold: ' +
                      threshold.threshold +
                      ' ' +
                      this.currentUnitChart
                  );
                }
                return returnValue;
              },
            },
          },
        },
      },
    });
  }

  handleGetData(
    transactionParam: IDashboardTransactionRequest | null,
    eventName?: string | null
  ) {
    if (eventName === 'click' && this.isStartSlideShow === true) {
      return;
    }
    const connectionId = this.signalRService.connectionId;
    if (this.dashboardType === DashboardType.SystemHealth) {
      this.isFirstTimeLoad = null;
      if (this.monitoringType !== MonitoringType.FreeDisk) {
        this.showCustomLegend = false;
      }
      this.isLoadingChart = true;
      this.currentIndexKPI = this.listMonitoringType.findIndex(
        (a) => a.id === this.monitoringType
      );
      let listNodeSummray = this.listNodes.filter((a) => a.selecting === true);
      let listNodeClone: {
        ID: number;
        environmentID: number;
        nodeName: string;
        environmentName: string;
      }[] = [...listNodeSummray].map((item) => {
        return {
          ID: item.id,
          environmentID: item.environmentID,
          nodeName: item.nodeName,
          environmentName: item.environmentName,
        };
      });
      this.selectedNodeName =
        this.listMonitoringType[this.currentIndexKPI].name;
      this.startHttpRequest({
        connectionId,
        systemHealthRequest: {
          connectionId,
          monitoringType: this.monitoringType,
          nodeSettings: listNodeClone,
        },
        transactionRequest: null,
      });
    } else if (this.dashboardType === DashboardType.TransactionBased) {
      this.startHttpRequest({
        connectionId,
        systemHealthRequest: null,
        transactionRequest: transactionParam,
      });
    }
  }

  onSelectingTabledata(selectedRow: INodeSettingDashboardHealthModel) {
    let item = this.listNodes.find(
      (a) =>
        a.environmentID === selectedRow.environmentID &&
        a.nodeName === selectedRow.nodeName
    );
    if (item) item.selecting = selectedRow.selecting;
    this.listNodes = this.listNodes.map((item) => {
      return { ...item, utilization: !item.selecting ? '' : item.utilization };
    });
    if (this.listNodes.findIndex((a) => !a.selecting) === -1) {
      this.isSelectAll = true;
      this.showButtonService = true;
    } else {
      this.isSelectAll = false;
      if (this.listNodes.findIndex((a) => a.selecting) !== -1)
        this.showButtonService = true;
      else {
        this.showButtonService = false;
        this.dataServiceWithInstanceCounter = [];
        this.dataServiceWithStatus = [];
      }
    }
    this.handleGetData(null);
  }

  onSelectAll() {
    if (!this.isSelectAll) this.showButtonService = false;
    else this.showButtonService = true;
    var cloneArray = [...this.listNodes].map((item) => {
      return {
        ...item,
        selecting: this.isSelectAll,
        utilization: !this.isSelectAll ? '' : item.utilization,
      };
    });
    this.listNodes = cloneArray;
    this.handleGetData(null);
  }

  onClickGetNode() {
    this.showButtonService = false;
    this.listNodes = this.listNodes.slice().map((item) => {
      return { ...item, selecting: false };
    });

    this.isLoading = true;
    this.isSelectAll = false;
    this.dataServiceWithInstanceCounter = [];
    this.dataServiceWithStatus = [];
    this._dashboardService.getListNodes().subscribe(
      (respone) => {
        this.listNodes = respone
          .filter((a) => a.nodeType === NodeType.SystemHealth)
          .map((item) => {
            return {
              ...item,
              selecting: item.isDefault === true ? true : false,
            };
          });
      },
      (err) => {
        this.isLoading = false;
      },
      () => {
        this.isLoading = false;
        this.showButtonService =
          this.listNodes.find((s) => s.selecting === true) !== undefined;
        this.handleGetData(null);
      }
    );
  }

  //0: By Status, 1: By Instance Counter
  handleGetDataServices(typeGetData: number) {
    if (typeGetData === 0) this.isLoadingServices = true;
    let dataBody: IServiceListRequestModel[] = [];
    let selectingNodes = this.listNodes
      .slice()
      .filter((n) => n.selecting === true);
    dataBody = selectingNodes.slice().map((item) => {
      return {
        enviromentName: item.environmentName,
        EnvironmentID: item.environmentID,
        MachineName: item.nodeName,
        NodeID: item.id,
      };
    });
    this._dashboardService.getListServices(dataBody).subscribe(
      (res) => {
        if (typeGetData === 0) this.dataServiceWithStatus = res;
        else if (typeGetData === 1) this.dataServiceWithInstanceCounter = res;
      },
      (err) => {
        this.isLoadingServices = false;
      },
      () => {
        if (typeGetData === 0) this.isLoadingServices = false;
      }
    );
  }

  drawChartFreeDisk(
    labels: string[],
    dataSets: { label: string; data: number[]; backgroundColor: string[] }[],
    annotation: AnnotationOptions[]
  ) {
    const ctx = this.freeDisk.nativeElement.getContext('2d');
    this.freeDiskChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: labels,
        datasets: dataSets,
      },
      plugins: [ChartDataLabels],
      options: {
        indexAxis: 'y',
        scales: {
          x: {
            beginAtZero: true,
            ticks: {
              autoSkip: false,
            },
            max: 100,
            min: 0,
            title: {
              display: true,
              text: '%',
            },
          },
        },
        plugins: {
          title: {
            display: false,
          },
          legend: {
            display: false,
          },
          datalabels: {
            formatter(value, context) {
              if (context.datasetIndex % 2 === 0) {
                if (value) return context.dataset.label;
                else return '';
              }
              return '';
            },
            anchor: 'center',
            align: 'center',
            color: 'black',
            labels: {
              title: {
                font: {
                  weight: 'normal',
                  size: 12,
                },
              },
              value: {
                color: 'black',
              },
            },
          },
          annotation: {
            annotations: annotation,
          },
          tooltip: {
            callbacks: {
              label: (tooltipItem) => {
                const machineName = tooltipItem.label;
                const threshold = this.thresholdFreeDisk.find(
                  (s) => s.machineName === machineName
                );
                let returnValue: string[] = [
                  tooltipItem.dataset.label +
                    ' : ' +
                    tooltipItem.parsed.x +
                    '%',
                ];
                if (threshold) {
                  returnValue.push('Threshold: ' + threshold.threshold + '%');
                }
                return returnValue;
              },
            },
          },
        },
        responsive: true,
        maintainAspectRatio: false,
      },
    });
  }

  disableKPI: boolean = false;

  onClickStartSlideShow() {
    let itemBody = document.body;
    itemBody.className = 'toggle-sidebar';
    if (!this.isStartSlideShow) {
      this._dashboardService.getDurationSlideShowDashboard().subscribe(
        (respone) => {
          this.intervalSlideKPI = respone * 1000;
        },
        (err) => {
          this.intervalSlideKPI = 10000;
        },
        () => {
          this.elem = document.documentElement;
          this.go_full_screen();
          if (this.dashboardType === DashboardType.SystemHealth) {
            this.myInterval = setInterval(() => {
              this.currentIndexKPI += 1;
              if (this.currentIndexKPI === this.listMonitoringType.length)
                this.currentIndexKPI = 0;
              this.monitoringType =
                this.listMonitoringType[this.currentIndexKPI].id;
              this.handleGetData(null);
            }, this.intervalSlideKPI);
          }
        }
      );
    } else {
      this.closeFullscreen();
      clearInterval(this.myInterval);
    }
    this.isStartSlideShow = !this.isStartSlideShow;
  }

  go_full_screen() {
    if (this.elem.requestFullscreen) {
      this.elem.requestFullscreen();
    } else if (this.elem.mozRequestFullScreen) {
      /* Firefox */
      this.elem.mozRequestFullScreen();
    } else if (this.elem.webkitRequestFullscreen) {
      /* Chrome, Safari and Opera */
      this.elem.webkitRequestFullscreen();
    } else if (this.elem.msRequestFullscreen) {
      /* IE/Edge */
      this.elem.msRequestFullscreen();
    }
  }

  closeFullscreen() {
    if (this._document.exitFullscreen) {
      this._document.exitFullscreen();
    } else if (this._document.mozCancelFullScreen) {
      /* Firefox */
      this._document.mozCancelFullScreen();
    } else if (this._document.webkitExitFullscreen) {
      /* Chrome, Safari and Opera */
      this._document.webkitExitFullscreen();
    } else if (this._document.msExitFullscreen) {
      /* IE/Edge */
      this._document.msExitFullscreen();
    }
  }

  listCustomLegend: { label: string; color: string }[] = [
    { label: 'Node A', color: 'red' },
    { label: 'Node B', color: 'blue' },
  ];

  getCustomLegend(info: {
    threshold: number;
    color: string;
    machineName: string;
  }) {
    return 'border-bottom: 8px solid ' + info.color + ';';
  }

  onClickCustomLegend(item: {
    threshold: number;
    color: string;
    machineName: string;
    labelClass: string;
    environmentID: number;
  }) {
    const className = item.labelClass as string;
    if (className.includes('active')) {
      item.labelClass = 'label-inactive';
      let lineInfo = this.lineFreeDiskInfo.find(
        (a) =>
          a.machineName === item.machineName &&
          a.environmentID === item.environmentID
      );
      if (lineInfo) lineInfo.active = false;
    }
    if (className.includes('inactive')) {
      item.labelClass = 'label-active';
      let lineInfo = this.lineFreeDiskInfo.find(
        (a) =>
          a.machineName === item.machineName &&
          a.environmentID === item.environmentID
      );
      if (lineInfo) lineInfo.active = true;
    }
    if (this.monitoringType === MonitoringType.FreeDisk) {
      this.freeDiskChart.options.plugins.annotation.annotations =
        this.lineFreeDiskInfo.slice().filter((a) => a.active === true);
      this.freeDiskChart.update();
    } else {
      this.lineChart.options.plugins.annotation.annotations =
        this.lineFreeDiskInfo.slice().filter((a) => a.active === true);
      this.lineChart.update();
    }
  }

  onChangeDashboardTypeTab(dashboardType: DashboardType) {
    if(dashboardType===DashboardType.SystemHealth && this.inItSystemhealth===false){
      this.inItSystemhealth=true;
      this.onClickGetNode();
    }
    this.dashboardType = dashboardType;
  }

  handleStartGetDataTransaction(returnObject: {
    isStart: boolean;
    filterValue: { environmentID: number; cipFlow: string; lastest: string };
  }) {
    const isStartGetData = returnObject.isStart;
    const transactionParam: IDashboardTransactionRequest = {
      cIPFlow: returnObject.filterValue.cipFlow,
      connectionId: this.signalRService.connectionId,
      environmentID: returnObject.filterValue.environmentID,
      lastest: returnObject.filterValue.lastest,
    };
    if (isStartGetData === true) {
      this.handleGetData(transactionParam);
    }
  }
}
