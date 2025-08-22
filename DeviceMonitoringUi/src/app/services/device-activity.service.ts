import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';  // need to learn about this

// Наш интерфейс для модели
import {DeviceActivity} from '../models/device-activity';

// можно расширить позже
export interface DeviceSummary {
  deviceId: string;
}

@Injectable({providedIn: 'root'})
export class DeviceActivityService {
  private apiUrl = 'http://172.31.252.124:5040/api/DeviceActivity'

  // Inject HttpClient into the constructor
  constructor(private http: HttpClient) {}

  // Получение всех устройств
  getAllDevices(): Observable<DeviceSummary[]> {
    console.log('Attempting to fetch data from:', this.apiUrl);
    //
    return this.http.get<DeviceSummary[]>(`${this.apiUrl}/devices`);
  }

  getActivitiesByDeviceId(deviceId: string): Observable<DeviceActivity[]> {
    return this.http.get<DeviceActivity[]>(`${this.apiUrl}/${deviceId}`);
  }
}
