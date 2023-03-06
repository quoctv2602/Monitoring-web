import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { globalsettings } from 'src/assets/globalsetting';
import { IDashboardSystemHealthModel } from '../_model/IDashboardSystemHealthModel';
import { IDashboardTransactionModel } from '../_model/IDashboardTransactionModel';
@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  subjectReceiveData = new BehaviorSubject<IDashboardSystemHealthModel[]>([]);
  subjectReceiveTransactionData =
    new BehaviorSubject<IDashboardTransactionModel>({
      columnChartData: [],
      tableData: [],
      pendingGraphData: [],
      thresholdPendingGraph: null,
    });
  subjectCompletedGetConnectionId = new BehaviorSubject<boolean>(false);
  public dataSystemhealth!: IDashboardSystemHealthModel[];
  public dataTransaction!: any;
  private hubConnection!: signalR.HubConnection;
  public connectionId!: string;
  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(globalsettings.apiUrl + 'chart', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .then(() => this.getConnectionId())
      .catch((err) => console.log('Error while starting connection: ' + err));
  };

  public addTransferChartDataListener = () => {
    this.hubConnection.on('transfersystemhealthdata', (data) => {
      this.dataSystemhealth = data;
      this.subjectReceiveData.next(data);
    });

    this.hubConnection.on('transfertransactiondata', (data) => {
      this.dataTransaction = data;
      this.subjectReceiveTransactionData.next(data);
    });
  };

  private getConnectionId = () => {
    this.hubConnection.invoke('getconnectionid').then((data: string) => {
      console.log(('Get ConnectionId Completed: ' + data) as string);
      this.connectionId = data;
      this.subjectCompletedGetConnectionId.next(true);
    });
  };

  closeConnection = () => {
    this.hubConnection.stop().then(() => {
      console.log('Connection Stopped');
      this.subjectCompletedGetConnectionId.next(false);
    });
  };
}
