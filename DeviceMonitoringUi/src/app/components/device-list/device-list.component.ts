import {Component, OnInit} from '@angular/core';
import {DeviceActivity} from 'src/app/models/device-activity';
import {DeviceActivityService} from 'src/app/services/device-activity.service';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.less']
})
export class DeviceListComponent implements OnInit {
  // Здесь хранится информация об устройствах, полученная от API
  devices: DeviceActivity[] = [];

  // Inject наш сервис в компонент
  constructor(private deviceActivityService: DeviceActivityService) {}

  //
  ngOnInit(): void {
    console.log('DeviceListComponent initialized');
    this.getDevices();
  }

  // Метод для получения информации от сервиса
  getDevices(): void {
    this.deviceActivityService.getAllDevices().subscribe({
      next: (data) => {
        this.devices = data;
        console.log('Devices received:', this.devices);
      },
      error: (error) => {
        console.error('Error occured.', error);
      }
    });
  }
}
