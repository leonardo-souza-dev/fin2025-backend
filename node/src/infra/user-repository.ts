import User from '../models/user';
import Sqlitedb from './sqlitedb'

export default class UserRepository {

    private db: Sqlitedb
    private tableName: string = 'users'

    constructor() {
        this.db = new Sqlitedb();
    }

    async create(entity: User): Promise<number> {
        let id = -1
        try {
            id = await this.db.create(this.tableName, entity)
        } catch (err: any) {
            throw new Error('🧥Error creating user: ' + err.message)
        }
        return id;
    }

    async getByEmail(email: string): Promise<User | null> {
        const users = await this.getAll()
        const user = users.find((user) => user.email === email)
        if (!user) {
            return null
        }
        return user
    }

    async getById(id: number): Promise<User | null> {
        const users = await this.getAll()
        const user = users.find((user) => user.id === id)
        if (!user) {
            return null
        }
        return user
    }

    async getAll(): Promise<User[]> {
        const users = await this.db.getAll<User>(this.tableName)
        return users
    }
}