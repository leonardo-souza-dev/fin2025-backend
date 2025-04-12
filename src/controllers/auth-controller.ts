import { Request, Response, Router } from 'express'
import Sqlitedb from '../infra/sqlitedb'
import { hashPassword, verifyPassword, generateAccessToken, generateRefreshToken } from '../../src/controllers/services/auth.service'
import User from '../models/user'
import UserRepository from '../infra/user-repository'
import jwt from 'jsonwebtoken'
import { authenticateToken } from '../middleware/auth.middleware'

export default class AuthController {
    
    private router: Router
    private db: Sqlitedb
    private userRepository: UserRepository = new UserRepository()

    constructor() {
        this.router = Router()
        this.db = new Sqlitedb()
        this.initializeRoutes()
    }

    private initializeRoutes() {
        this.router.post('/register', this.register.bind(this))
        this.router.post('/login', this.login.bind(this))
        this.router.post('/refresh', this.refreshToken.bind(this))
        this.router.post('/logout', this.logout.bind(this))
        this.router.get("/dados-seguros", authenticateToken, (req, res) => {
            res.json({ message: "This is a protected route!" })
        })
    }

    private async register(req: Request, res: Response) {
        try {
            const { email, password } = req.body

            const existingUser = await this.userRepository.getByEmail(email)
            if (existingUser) {
                res.status(400).json()
                return
            }

            const hashedPassword = await hashPassword(password)
            
            const newUser: User = new User(email, hashedPassword, true, null)

            try {
                await this.userRepository.create(newUser)
            } catch (err: any) {
                res.status(500).json({ error: err.message })
                return
            }

            res.status(201).json({ message: "User registered" })
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    private async login(req: Request, res: Response) {
        try {
            const { email, password } = req.body

            const user = await this.userRepository.getByEmail(email)

            if (!user) {
                res.status(401).json({ message: "User not found" })
                return
            }
            if (!(await verifyPassword(password, user.password))) {
                res.status(401).json({ message: "Credenciais inválidas" })
                return
            }

            const accessToken = generateAccessToken(user)

            const refreshToken = generateRefreshToken(user)

            res.cookie("refreshToken", refreshToken, {
                httpOnly: true,
                secure: true,
                sameSite: "strict"
            })

            res.json({ accessToken })
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    private async logout(req: Request, res: Response) {
        try {
            res.clearCookie("refreshToken")
            res.json({ message: "Logout success" })
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    private async refreshToken(req: Request, res: Response) {
        try {
            const refreshToken = req.cookies.refreshToken

            if (!refreshToken) {
                res.status(401).json({ message: "Not authorized" })
                return
            }

            let decoded: any;
            try {
                decoded = jwt.verify(refreshToken, process.env.JWT_REFRESH_SECRET as string, { ignoreExpiration: true });
            } catch (err: any) {
                res.status(401).json({ message: "Invalid token" });
                return;
            }


            if (!decoded || !(decoded as any).id) {
                res.status(401).json({ message: "Invalid token payload" })
                return;
            }

            const user = await this.userRepository.getById((decoded as any).id)
            if (!user) {
                res.status(401).json({ message: "User not found" })
                return
            }

            const newRefreshToken = generateRefreshToken(user)
            res.cookie("refreshToken", newRefreshToken, {
                httpOnly: true,
                secure: true,
                sameSite: "strict"
            })

            const newAccessToken = generateAccessToken(user)
            res.json({ accessToken: newAccessToken });
        } catch (err: any) {
            res.status(500).json({ error: err.message })
        }
    }

    public getRouter() {
        return this.router
    }
}
