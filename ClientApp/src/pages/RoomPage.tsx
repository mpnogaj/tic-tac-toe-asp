import { ParamsComponent, ParamsComponentProps, paramsHOC } from '@/components/hoc/ParamsComponent';
import Endpoints, { Base } from '@/endpoints';
import Player from '@/types/dto/player';
import Room from '@/types/dto/room';
import { empty } from '@/types/other';
import * as signalR from '@microsoft/signalr';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';
import axios from 'axios';
import React from 'react';

import GameComponent from '../components/GameComponent';
import player from '@/types/dto/player';

type RoomPageState = {
	isLoading: boolean;
	connectionClosed: boolean;
	error: string | undefined;
	gameStarted: boolean;
	playersReady: Map<string, boolean>;
	players: Map<string, Player>;
	roomName: string;
};

type RoomPageParams = {
	roomGuid: string;
};

class RoomPage extends ParamsComponent<empty, RoomPageParams, RoomPageState> {
	connection: HubConnection;

	startingPlayer: Player | undefined;

	constructor(props: ParamsComponentProps<empty, RoomPageParams>) {
		super(props);
		this.state = {
			isLoading: true,
			connectionClosed: false,
			error: undefined,
			gameStarted: false,
			playersReady: new Map<string, boolean>(),
			players: new Map<string, Player>(),
			roomName: ''
		};

		this.startingPlayer = undefined;
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(`${Base}/game?room=${props.params.roomGuid}`)
			.build();

		this.connection.on('NotifyToggleReady', (guid: string, ready: boolean) => {
			this.setState({
				...this.state,
				playersReady: new Map(this.state.playersReady.set(guid, ready))
			});
		});
		this.connection.on('NotifyGameStarted', (player: Player) => {
			this.startingPlayer = player;
			this.setState({ ...this.state, gameStarted: true });
		});
		this.connection.on('NotifyPlayerJoined', (player: Player) => {
			this.setState({
				...this.state,
				players: new Map(this.state.players.set(player.guid, player))
			});
		});
		this.connection.onclose(() => {
			this.setState({ connectionClosed: true });
		});
		this.connection.on('NotifyPlayerLeft', playerGuid => {
			if (this.state.gameStarted) return;
			this.state.players.delete(playerGuid);
			this.state.playersReady.delete(playerGuid);

			this.setState({
				...this.state,
				players: new Map(this.state.players),
				playersReady: new Map(this.state.playersReady)
			});
		});
	}

	async componentDidMount(): Promise<void> {
		if (this.connection.state === HubConnectionState.Disconnected) {
			try {
				const res = await this.connection.start();

				console.log(res);

				const resp = await axios.get<Room>(`${Endpoints.Rooms}/${this.props.params.roomGuid}`);
				const room = resp.data;
				const playersMap = new Map(room.players.map(x => [x.guid, x]));

				const readyMap = new Map(
					Object.keys(room.readyDict).map(key => {
						return [key, room.readyDict[key]];
					})
				);

				this.setState({
					...this.state,
					isLoading: false,
					error: undefined,
					players: playersMap,
					playersReady: readyMap,
					roomName: room.roomName
				});
			} catch (err) {
				this.setState({ ...this.state, isLoading: false, error: err });
			}
		}
	}

	componentWillUnmount(): void {
		if (this.connection.state === HubConnectionState.Connected) {
			//fire and forget
			this.connection.stop().then().catch();
		}
	}

	render(): React.ReactNode {
		if (this.state.isLoading) {
			return <div>Connecting...</div>;
		}

		if (this.state.connectionClosed) {
			return (
				<div className="container d-flex">
					<div className="mt-3 mx-auto alert alert-danger d-inline-block" role="alert">
						<div className="text-center">
							Connection closed. You might have passed wrong room id. <a href="/rooms">Go back to room list</a>
						</div>
					</div>
				</div>
			);
		}

		if (this.state.connectionClosed || this.state.error !== undefined) {
			const error = this.state.error ?? 'Invalid room guid';
			return (
				<div className="container d-flex">
					<div className="mt-3 mx-auto alert alert-danger d-inline-block" role="alert">
						<div className="text-center">
							Connection closed. Error: {error}. <a href="/rooms">Go back to room list</a>
						</div>
					</div>
				</div>
			);
		}

		if (this.state.gameStarted && this.startingPlayer !== undefined) {
			return (
				<GameComponent startingPlayer={this.startingPlayer} socket={this.connection} />
			);
		} else {
			return (
				<div className="container">
					<h1>Room: {this.state.roomName}</h1>
					<h2>Room code: {this.props.params.roomGuid}</h2>
					<h2>Players:</h2>
					<div className="mt-3 mb-3">
						<ul className="list-group d-inline-flex">
							{Array.from(this.state.players.values()).map(player => {
								const isReady = this.state.playersReady.get(player.guid) ?? false;
								return (
									<li className="list-group-item d-flex justify-content-between align-items-center" key={player.guid}>
										{player.nickname}
										<span className="badge rounded-pill">{isReady ? '✔' : '❌'}</span>
									</li>
								);
							})}
						</ul>
					</div>

					<button
						className="btn btn-primary"
						onClick={async () => {
							await this.connection.invoke('ToggleReady');
						}}
					>
						Toggle ready
					</button>
				</div>
			);
		}
	}
}

export default paramsHOC(RoomPage);
