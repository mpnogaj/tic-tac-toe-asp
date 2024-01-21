import { NavComponent, NavComponentProps, navHOC } from '@/components/hoc/NavComponent';
import Room from '@/types/dto/room';
import { empty } from '@/types/other';
import axios from 'axios';
import React from 'react';

import RoomComponent from '../components/RoomComponent';
import Endpoints, { Base } from '../endpoints';
import { isAuth } from '@/utils/auth';

interface ILoginPageState {
	nickname: string | undefined;
	isLoggedIn: boolean;
	isRefreshing: boolean;
	roomName: string;
	rooms: Room[];
}

class RoomsPage extends NavComponent<empty, ILoginPageState> {
	constructor(props: NavComponentProps<empty>) {
		super(props);

		this.state = {
			nickname: undefined,
			isLoggedIn: false,
			isRefreshing: false,
			roomName: '',
			rooms: []
		};
	}

	componentDidMount(): void {
		this.fetchNickname();
		this.fetchRooms();
		this.isLoggedIn();
	}

	fetchRooms = async () => {
		console.log('Refreshing rooms');

		try {
			this.setState({ isRefreshing: true });
			const resp = await axios.get<Array<Room>>(`${Base}${Endpoints.Rooms}`, {
				withCredentials: true
			});
			this.setState({ rooms: resp.data, isRefreshing: false });
		} catch (error) {
			console.error(error);
		}
	};

	fetchNickname = async () => {
		try {
			const nickname = (await axios.get<string>(Endpoints.Player)).data;
			this.setState({ ...this.state, nickname: nickname });
		} catch (err) {
			console.error(err);
		}
	};
	
	isLoggedIn = async () => {
		try {
			const isLoggedIn = await isAuth();
			this.setState({...this.state, isLoggedIn: isLoggedIn});
		} catch (err) {
			console.error(err);
		}
	}

	logoutHandler = async () => {
		try {
			await axios.post(Endpoints.PlayerLogout);
			this.props.navigate('/login');
		} catch (err) {
			console.error(err);
		}
	};

	createRoom = async () => {
		const roomName = this.state.roomName;

		const room: Room = {
			roomName: roomName,
			maxPlayerCount: 2,
			guid: '00000000-0000-0000-0000-000000000000',
			players: [],
			readyDict: {}
		};

		try {
			const resp = await axios.post<Room>(`${Base}${Endpoints.Rooms}`, room);
			const newRoom = resp.data;
			console.log(newRoom);
			this.props.navigate(`/room/${newRoom.guid}`);
		} catch (err) {
			alert("Error. Couldn't create room. Check console for details");
			console.error(err);
		}
	};

	joinRoom = async (roomGuid: string) => {
		try {
			await axios.post(`${Endpoints.Rooms}/${roomGuid}`);
			this.props.navigate(`/room/${roomGuid}`);
		} catch (err) {
			console.error(err);
		}
	};

	render(): React.ReactNode {
		return (
			<div className="container">
				<div className="row mt-3">
					<div className="col">
						<h2>Hi {this.state.nickname ?? ''}</h2>
					</div>
					<div className="col-auto">
						<button className="btn btn-primary" onClick={async () => await this.logoutHandler()} disabled={!this.state.isLoggedIn}>Profile</button>
					</div>
					<div className="col-auto">
						<button className="btn btn-primary" onClick={async () => await this.logoutHandler()}>Logout</button>
					</div>
				</div>

				<h1>Rooms</h1>
				<div className="row">
					<div className="col-auto">
						<button
							className="btn btn-primary"
							data-bs-toggle="modal" data-bs-target="#createRoomModal"
						>
							Create room
						</button>
					</div>
					<div className="col-auto">
						<button
							disabled={this.state.isRefreshing}
							className="btn btn-primary"
							onClick={async () => {
								await this.fetchRooms();
							}}
						>
							{this.state.isRefreshing ? 'Refreshing...' : 'Refresh'}
						</button>
					</div>
				</div>
				<div>
					<RoomComponent
						room={{roomName: 'test', guid: '0000-00-123', players: [
								{nickname: 'DiscoDzban', guid: '123-12345678-1234'},
								{nickname: 'Turbo Lover', guid: '123-6969696969-2137'}
							], readyDict: {}, maxPlayerCount: 2}}
						joinRoomCallback={room => {}}
						key='123'
					/>
					<RoomComponent
						room={{roomName: 'XxXxXxX_XxXxXxX_XxXxXxX', guid: '0000-00-123', players: [
								{nickname: 'DiscoKarton123', guid: '123-12345678-1234'},
							], readyDict: {}, maxPlayerCount: 2}}
						joinRoomCallback={room => {}}
						key='456'
					/>
					
					{this.state.rooms.map(room => {
						return (
							<RoomComponent
								room={room}
								joinRoomCallback={async (room) => {
									await this.joinRoom(room.guid);
								}}
								key={room.guid}
							/>
						);
					})}
				</div>
				
				<div className="modal fade" id="createRoomModal" tabIndex={-1} role="dialog" aria-labelledby="createRoomModalLabel" aria-hidden="true">
					<div className="modal-dialog" role="document">
						<div className="modal-content">
							<div className="modal-header">
								<h5 className="modal-title" id="exampleModalLabel">Create room</h5>
								<button type="button" className="close" data-bs-dismiss="modal" aria-label="Close">
									<span aria-hidden="true">&times;</span>
								</button>
							</div>
							<div className="modal-body">
								<form>
									<div className="form-group">
										<label>Room name: </label>
										<input
											className="form-control"
											type="text"
											value={this.state.roomName}
											onInput={e => {
												this.setState({ roomName: e.currentTarget.value });
											}}
										/>
									</div>
								</form>
							</div>
							<div className="modal-footer">
								<button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
								<button type="button" className="btn btn-primary" data-bs-dismiss="modal" disabled={this.state.roomName === ''}  onClick={async () => {
									await this.createRoom()
								}}>Create room</button>
							</div>
						</div>
					</div>
				</div>
			</div>
		);
	}
}

export default navHOC(RoomsPage);
