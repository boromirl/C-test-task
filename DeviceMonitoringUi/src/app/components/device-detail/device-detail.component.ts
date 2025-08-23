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

  deleteActivity(activityId: string): void {
    if (!confirm('Are you sure you want to delete this activity?')) {
      return;  // при отмене удаления
    }

    this.deviceActivityService.deleteActivity(activityId).subscribe({
      next: () => {
        // Удаляем activity из массива в фронтэнде
        this.activities =
            this.activities.filter(a => a.activityId !== activityId);
        console.log('Activity deleted successfully');

        // Если не осталось activities
        // if (this.activities.length == 0) {
        //
        // }
      },
      error: (error) => {
        console.error('Error deleting activity:', error);
        alert('Failed to delete activity. Please try again');
      }
    });
  }
}
