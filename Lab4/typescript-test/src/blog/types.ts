import type { Article } from "./Article";
import type { Author } from "./Author";
import type { Blog } from "./Blog";

export type CreateAuthorDto = Omit<Author, "id">;
export type CreateBlogDto = Pick<Blog, "name" | "author">;
export type CreateArticleDto = Pick<Article, "title" | "content">;

export type AuthorListItem = Pick<Author, "firstName" | "lastName">;

export type ReadonlyAuthors = Readonly<Readonly<Author>[]>;
