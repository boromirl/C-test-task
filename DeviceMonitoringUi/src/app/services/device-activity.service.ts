import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable, tap} from 'rxjs';  // need to learn about this

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
    return this.http.get<DeviceSummary[]>(`${this.apiUrl}/devices`).pipe(tap({
      next: (devices) => console.log(`Retrieved ${devices.length} devices`),
      error: (error) => console.error('Error fetching devices:', error)
    }));
  }

  getActivitiesByDeviceId(deviceId: string): Observable<DeviceActivity[]> {
    console.log('Attempting to fetch list of activities of device: ', deviceId);

    return this.http.get<DeviceActivity[]>(`${this.apiUrl}/${deviceId}`)
        .pipe(tap({
          next: (activities) =>
              console.log(`Retrieved ${activities.length} activities`),
          error: (error) => console.error('Error fetching activities:', error)
        }));
  }

  deleteActivity(activityId: string): Observable<void> {
    console.log('Attempting to delete activity:', activityId);

    return this.http.delete<void>(`${this.apiUrl}/activity/${activityId}`)
        .pipe(tap({
          next: () =>
              console.log(`Successfully deleted activity ${activityId}`),
          error: (error) => console.error('Error deleting activity', error)
        }));
  }
}
