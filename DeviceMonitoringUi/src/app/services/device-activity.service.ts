import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';  // need to learn about this

// Наш интерфейс для модели
import {DeviceActivity} from '../models/device-activity';

@Injectable({providedIn: 'root'})
export class DeviceActivityService {
  private apiUrl = 'http://172.31.252.124:5040/api/DeviceActivity'

  // Inject HttpClient into the constructor
  constructor(private http: HttpClient) {}

  // Получение всех устройств
  getAllDevices(): Observable<DeviceActivity[]> {
    console.log('Attempting to fetch data from:', this.apiUrl);
    // Отправка GET реквеста к API. Ожидает массив DeviceActivity в ответ
    return this.http.get<DeviceActivity[]>(this.apiUrl);
  }

  getDeviceById(id: string): Observable<DeviceActivity> {
    return this.http.get<DeviceActivity>(`${this.apiUrl}/${id}`);
  }
}
