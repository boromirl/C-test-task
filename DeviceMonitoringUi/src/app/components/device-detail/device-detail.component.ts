import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {DeviceActivity} from 'src/app/models/device-activity';
import {DeviceActivityService} from 'src/app/services/device-activity.service';

@Component({
  selector: 'app-device-detail',
  templateUrl: './device-detail.component.html',
  styleUrls: ['./device-detail.component.less']
})
export class DeviceDetailComponent implements OnInit {
  activities: DeviceActivity[] = [];
  deviceId: string|null = null;

  constructor(
      private route: ActivatedRoute,
      private deviceActivityService: DeviceActivityService) {}

  ngOnInit(): void {
    this.deviceId = this.route.snapshot.paramMap.get('id');
    // null check для ID, чтобы избежать ошибок, если ID не найдено в URL
    if (this.deviceId) {
      this.getActivities();
    }
  }

  getActivities(): void {
    if (!this.deviceId) return;

    this.deviceActivityService.getActivitiesByDeviceId(this.deviceId)
        .subscribe({
          next: (data) => {
            this.activities = data;
            console.log('Activities received:', this.activities);
          },
          error: (error) => {
            console.error('Error occurred', error);
          }
        })
  }
}
