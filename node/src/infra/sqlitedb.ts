import sqlite3 from 'sqlite3'
import Entity from '../models/entity'

class Sqlitedb {

    private database: sqlite3.Database

    constructor() {
        this.database = new sqlite3.Database('./fin_db.db', (err: any) => {
            if (err) {
                console.error(err.message)
            }
        })
    }

    async getAll<T>(tableName: string): Promise<T[]> {
        const { error, rows } = await this.select(`SELECT * FROM ${tableName}`)
        if (error) {
            return []
        }
        return rows as T[]
    }

    async getById<T>(tableName: string, id: number): Promise<T> {
        const { error, rows } = await this.select(`SELECT * FROM ${tableName} WHERE id = ?`, [id])
        if (error) {
            return {} as T
        }
        return rows ? (rows[0] as T) : ({} as T)
    }

    private async select(sql: string, params: any[] = []): Promise<{ error?: Error; rows?: any[] }> {
        return new Promise(resolve => {
            this.database.all(sql, ...params, (error: any, row: any) => {
                error ? resolve({ error }) : resolve({ rows: row || [] })
            })
        })
    }

    async create<T>(tableName: string, data: Entity<T>): Promise<number> {

        if (!data) {
            throw new Error('data is required')
        }

        const id : number = await this.getNextId(tableName)

        if (id === undefined || id === null) {
            throw new Error('id is required')
        }

        data.id = id

        const keys : string[] = Object.keys(data)
        const values : any[] = Object.values(data)

        const sql : string = `INSERT INTO ${tableName} (${keys.join(', ')}) VALUES (${keys.map(() => '?').join(', ')})`

        await this.run(sql, values)
        const idInserted : { error?: Error; rows?: any[]; } = await this.select(`SELECT rowid from ${tableName} order by ROWID DESC limit 1`)
        
        if (idInserted.rows && idInserted.rows.length > 0) {
            return idInserted.rows[0].id
        } else {
            throw new Error('idInserted.rows is empty')
        }
    }

    async getNextId(tableName: string): Promise<number> {
        const { error, rows } = await this.select(`SELECT seq FROM sqlite_sequence WHERE name = ?`, [tableName])

        if (error) {
            console.error(error)
            return 0
        }
        const result = rows && rows.length > 0 ? rows[0].seq + 1 : 1

        return result
    }

    async update<T>(tableName: string, data: Entity<T>): Promise<void> {
        const keys : string[] = Object.keys(data)
        const values : any[] = Object.values(data)
        const sql : string = `UPDATE ${tableName} SET ${keys.map((key) => `${key} = ?`).join(', ')} WHERE id = ${data.id}`
        await this.run(sql, values)        
    }

    async delete(tableName: string, id: number): Promise<void> {
        await this.run(`DELETE FROM ${tableName} WHERE id = ?`, [id])
    }

    private async run(sql: string, values: any[]): Promise<void> {
        return new Promise(resolve => {
            const db = this.database.run(sql, values, (error: Error | null) => {
                error ? console.error(error) : resolve()
            })
        })
    }
}
export default Sqlitedb