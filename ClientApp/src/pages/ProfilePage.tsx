import LoadingComponent from '@/components/LoadingComponent';
import Endpoints from '@/endpoints';
import Account from '@/types/dto/account';
import { empty } from '@/types/other';
import axios from 'axios';
import React from 'react';

type CompState = {
	error: string | undefined;
	accountInfo: Account | undefined;
	nickname: string;
	newPassword: string;
	newPasswordConfirm: string;
	oldPassword: string;
};

class ProfilePage extends React.Component<empty, CompState> {
	constructor(props: empty) {
		super(props);

		this.state = {
			error: undefined,
			accountInfo: undefined,
			nickname: '',
			newPassword: '',
			newPasswordConfirm: '',
			oldPassword: ''
		};
	}

	async componentDidMount(): Promise<void> {
		try {
			const accountInfo = (await axios.get<Account>(Endpoints.Auth)).data;
			const nickname = (await axios.get<string>(Endpoints.Player)).data;

			this.setState({
				...this.state,
				accountInfo: accountInfo,
				nickname: nickname
			});
		} catch (err) {
			let errorMsg = 'Unknown error';
			if (err instanceof Error) {
				errorMsg = err.message;
			}
			console.error(errorMsg);
			this.setState({ ...this.state, error: errorMsg });
		}
	}

	passwordsMatches = () => {
		return this.state.newPassword === this.state.newPasswordConfirm;
	};

	submitHandler = async () => {
		if (!this.passwordsMatches()) return;

		try {
			await axios.patch(Endpoints.Auth, {
				nickname: this.state.nickname,
				newPassword: this.state.newPassword,
				oldPassword: this.state.oldPassword
			});
		} catch (err) {
			console.error(err);
		}
	};

	render() {
		if (this.state.error !== undefined) {
			return (
				<div>
					<h1>Error</h1>
					{this.state.error}
				</div>
			);
		}

		if (this.state.accountInfo === undefined) {
			return <LoadingComponent />;
		}

		return (
			<div className="container">
				<h1>Your account - {this.state.accountInfo.username}</h1>
				<a className="btn btn-primary" href="/">Back</a>
				<div className="mt-2 mb-2">
					<h2>Edit data</h2>
					<div className="form">
						<div className="mb-3">
							<label className="form-label">Nickname</label>
							<input
								className="form-control"
								type="text"
								value={this.state.nickname}
								onChange={e => {
									this.setState({ ...this.state, nickname: e.target.value });
								}}
							/>
						</div>

						<div className="mb-3">
							<label className="form-label">New password (leave blank if you don&apos;t want to edit)</label>
							<input
								className="form-control"
								type="password"
								value={this.state.newPassword}
								onChange={e => {
									this.setState({ ...this.state, newPassword: e.target.value });
								}}
							/>
						</div>
						<div className="mb-3">
							<label className="form-label">Confirm new password</label>
							<input
								className="form-control"
								type="password"
								value={this.state.newPasswordConfirm}
								onChange={e => {
									this.setState({ ...this.state, newPasswordConfirm: e.target.value });
								}}
							/>
							<span
								style={{
									color: 'red',
									display: this.passwordsMatches() ? 'none' : 'block'
								}}
							>
								Password doesn&apos;t match
							</span>
						</div>

						<div className="mb-3">
							<label className="form-label">Current password</label>
							<input
								className="form-control"
								type="password"
								value={this.state.oldPassword}
								onChange={e => {
									this.setState({ ...this.state, oldPassword: e.target.value });
								}}
							/>
						</div>
						<button className="btn btn-primary" onClick={async () => await this.submitHandler()}>
							Submit
						</button>
					</div>
				</div>
				<div className="mt-4 mb-2">
					<h2>Stats</h2>
					<div>
						<span>Matches played: {this.state.accountInfo.matchesPlayed}</span>
					</div>
					<div>
						<span>Matches won: {this.state.accountInfo.matchesWon}</span>
					</div>
				</div>
			</div>
		);
	}
}

export default ProfilePage;
