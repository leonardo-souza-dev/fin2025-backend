import bcrypt from 'bcryptjs'
import jwt, { Secret, SignOptions } from 'jsonwebtoken'
import { StringValue } from 'ms'
import dotenv from 'dotenv'
import User from '../models/user'

dotenv.config()

export const hashPassword = async (password: string): Promise<string> => {
    const salt = await bcrypt.genSalt(10)
    return bcrypt.hash(password, salt)
}

export const verifyPassword = async (inputPassword: string, storedHash: string): Promise<boolean> => {
    return bcrypt.compare(inputPassword, storedHash)
}

export const generateAccessToken = (user: User): string => {
    const payload: object = { id: user.id, email: user.email }
    const secretOrPrivateKey = process.env.JWT_SECRET as Secret
    const options: SignOptions = { expiresIn: process.env.TOKEN_EXPIRES_IN as StringValue }
    
    return jwt.sign(payload, secretOrPrivateKey, options);
}

export const generateRefreshToken = (user: User): string => {
    const payload: object = { id: user.id }
    return jwt.sign(
        payload, 
        process.env.JWT_REFRESH_SECRET as string, 
        { expiresIn: process.env.REFRESH_TOKEN_EXPIRES_IN as StringValue }
    )
}
