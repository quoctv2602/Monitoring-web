import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { globalsettings } from 'src/assets/globalsetting';
@Injectable()
export class ConfigService {
  private config: any;
  private configUrl = globalsettings.configUrl;
  constructor(private http: HttpClient) {}
  load() {
    return this.http
      .get(this.configUrl + 'StaticFiles/appsettings.json')
      .toPromise()
      .then((config: any) => {
        this.config = config;
      });
  }
  get(key: string) {
    return this.config[key];
  }
}
