import { Component, OnDestroy, OnInit } from '@angular/core';
import * as globalEnum from './_common/_enum';

@Component({
  template: '',
})
export abstract class BaseComponent implements OnInit, OnDestroy {
  roles: Array<number> = [];

  constructor() {}

  ngOnInit(): void {
    if (localStorage.getItem('storage_p')) {
      this.roles = JSON.parse(localStorage.getItem('storage_p') as string);
    }
  }

  ngOnDestroy(): void {}

  get globalEnumResult(): typeof globalEnum {
    return globalEnum;
  }


}
