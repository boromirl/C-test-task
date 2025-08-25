import {Component, OnInit} from '@angular/core';
import {DeviceActivity} from 'src/app/models/device-activity';
import {DeviceActivityService, DeviceSummary} from 'src/app/services/device-activity.service';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.less']
})
export class DeviceListComponent implements OnInit {
  // Здесь хранится информация об устройствах, полученная от API
  devices: DeviceSummary[] = [];

  // Inject наш сервис в компонент
  constructor(private deviceActivityService: DeviceActivityService) {}

  //
  ngOnInit(): void {
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

  createBackup(): void {
    if (!confirm('Создать резервную копию? (Подтвердите действие)')) {
      return;
    }

    this.deviceActivityService.createBackup().subscribe({
      next: (response) => {
        alert('Резервная копия данных успешно создана!');
        console.log('Backup response:', response);
      },
      error: (error) => {
        console.error('Error creating backup:', error);
        alert(
            'Не удалось создать резервную копию. Пожалуйста, попробуйте ещё раз.');
      }
    });
  }

  restoreFromBackup(): void {
    if (!confirm(
            'Восстановить данные из резервной копии? (Это заменит все текущие данные!)')) {
      return;
    }

    this.deviceActivityService.restoreFromBackup().subscribe({
      next: (response) => {
        alert('Данные успешно восстановлены!');
        console.log('Restore response:', response);
        // обновление списка
        this.getDevices();
      },
      error: (error) => {
        console.error('Error restoring from backup:', error);
        alert(
            'Не удалось восстановить данные. Пожалуйста, попробуйте ещё раз.');
      }
    });
  }
}
