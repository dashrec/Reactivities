import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr';
import { makeAutoObservable, runInAction } from 'mobx';
import { ChatComment } from '../models/comment';
import { store } from './store';

export default class CommentStore {
  comments: ChatComment[] = [];
  hubConnection: HubConnection | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  createHubConnection = (activityId: string) => {
    // connect to a particular activity
    if (store.activityStore.selectedActivity) {
      // get selected activity

      this.hubConnection = new HubConnectionBuilder()
        .withUrl(process.env.REACT_APP_CHAT_URL +'/?activityId=' + activityId, {
          accessTokenFactory: () => store.userStore.user?.token!, //pass token. user can technically be null so use exclamation mark
        })
        .withAutomaticReconnect() // to reconnect our client to the chat hub if for whatever reason they lose their connection
        .configureLogging(LogLevel.Information) // to see whats going on as we connect
        .build(); // create connection

      this.hubConnection
        .start() // start connection
        .catch((error) =>
          console.log('Error establishing connection: ', error)
        );

      // when we create our connection to our hub, then we're going to want to receive all of the comments for that activity that live connected to
      this.hubConnection.on('LoadComments', (comments: ChatComment[]) => { // comments coming from db
        // LoadComments must match what we called in signalR ChatHub
        runInAction(() => {
          comments.forEach((comment) => {
            comment.createdAt = new Date(comment.createdAt); // update observable
          });
          this.comments = comments; // update observable
        });
      });

      this.hubConnection.on('ReceiveComment', (comment) => { //comments from hub
        runInAction(() => {
          comment.createdAt = new Date(comment.createdAt);
           this.comments.unshift(comment); // sets comment at the start of the array 

          //this.comments.push(comment); // sets comment at the end of the array 
        });
      });
    }
  };

  stopHubConnection = () => {
    this.hubConnection
      ?.stop()
      .catch((error) => console.log('Error stopping connection: ', error));
  };

  clearComments = () => {
    // it will clean up store when we move away from that activity
    this.comments = [];
    this.stopHubConnection();
  };

  addComment = async (values: any) => {
    values.activityId = store.activityStore.selectedActivity?.id; // get selected activity values and send to backend
    try {
      await this.hubConnection?.invoke('SendComment', values); // this needs to match exactly the name of the ChatHub method name
    } catch (error) {
      console.log(error);
    }
  };
}
