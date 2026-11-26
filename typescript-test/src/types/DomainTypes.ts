import { Author } from "../classes/author";
import { Article } from "../classes/article";
import { Blog } from "../classes/blog";

export type AuthorDto = Omit<Author, "id">;
export type AuthorSignature = Pick<Author, "firstName" | "lastName">;
export type ArticlePreview = Pick<Article, "id" | "title">;
export type CreateArticleDto = Omit<Article, "id">;
export type BlogHeader = Pick<Blog, "id" | "name" | "author">;
export type BlogWithoutArticles = Omit<Blog, "articles">;
export type AuthorsById = Record<number, Author>;
